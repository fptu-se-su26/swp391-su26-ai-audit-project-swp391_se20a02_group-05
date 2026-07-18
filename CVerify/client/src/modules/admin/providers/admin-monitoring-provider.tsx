"use client";

import React, { useEffect, useRef } from 'react';
import { HubConnectionBuilder, HttpTransportType, LogLevel, type HubConnection } from '@microsoft/signalr';
import { useAuthStore } from '@/features/auth/store/use-auth-store';
import { NotificationHub as ClientToastHub } from '@/infrastructure/notifications/orchestrator';
import { API_URL, axiosClient } from '@/infrastructure/http/axios-client';

type ToastCategory = 'info' | 'success' | 'warning' | 'error';

interface AdminMonitoringAlert {
  id: string;
  eventType: string;
  severity: string;
  source: string;
  message: string;
  createdAt: string;
}

const severityToCategory = (severity: string): ToastCategory => {
  switch ((severity || '').toLowerCase()) {
    case 'critical':
    case 'error':
      return 'error';
    case 'warning':
      return 'warning';
    default:
      return 'info';
  }
};

const normalizeAlert = (raw: any): AdminMonitoringAlert => ({
  id: raw.id ?? raw.Id ?? '',
  eventType: raw.eventType ?? raw.EventType ?? 'MONITORING_EVENT',
  severity: raw.severity ?? raw.Severity ?? 'info',
  source: raw.source ?? raw.Source ?? 'CVerify.AI',
  message: raw.message ?? raw.Message ?? '',
  createdAt: raw.createdAt ?? raw.CreatedAt ?? new Date().toISOString(),
});

/**
 * Subscribes admins to realtime monitoring alerts broadcast by Core (originating from
 * CVerify.AI) and raises a toast for each one. Mounted inside the ADMIN-guarded layout.
 */
export function AdminMonitoringProvider({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const user = useAuthStore((s) => s.user);
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    if (!isAuthenticated || !user) {
      if (connectionRef.current) {
        connectionRef.current.stop();
        connectionRef.current = null;
      }
      return;
    }

    const hubUrl = API_URL.replace(/\/api$/, '') + '/hubs/admin';

    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        withCredentials: true,
        transport: HttpTransportType.WebSockets | HttpTransportType.LongPolling,
        accessTokenFactory: async () => {
          // Ensure the access-token cookie is fresh (axios interceptors silently refresh).
          try {
            await axiosClient.get('/auth/me');
          } catch {
            // If unauthenticated the hub connection will fail with 401 anyway.
          }
          return '';
        },
      })
      .configureLogging(process.env.NODE_ENV === 'development' ? LogLevel.Information : LogLevel.Warning)
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveMonitoringAlert', (payload: unknown) => {
      try {
        const alert = normalizeAlert(payload);
        const category = severityToCategory(alert.severity);
        const title = `Monitoring: ${alert.eventType.replace(/^MONITORING_/, '').replace(/_/g, ' ')}`;
        ClientToastHub.dispatch({
          category,
          title,
          description: `[${alert.source}] ${alert.message}`,
        });
      } catch (err) {
        console.error('[AdminMonitoring] Failed to handle monitoring alert', err);
      }
    });

    connectionRef.current = connection;

    let isCancelled = false;
    let startPromise: Promise<void> | null = null;

    const start = async () => {
      try {
        startPromise = connection.start();
        await startPromise;
      } catch (err) {
        if (!isCancelled) {
          console.error('[AdminMonitoring] Connection failed:', err);
        }
      }
    };

    start();

    return () => {
      isCancelled = true;
      if (connectionRef.current) {
        const conn = connectionRef.current;
        connectionRef.current = null;
        if (startPromise) {
          startPromise.then(() => conn.stop()).catch(() => { /* disconnecting anyway */ });
        } else {
          conn.stop();
        }
      }
    };
  }, [isAuthenticated, user]);

  return <>{children}</>;
}
