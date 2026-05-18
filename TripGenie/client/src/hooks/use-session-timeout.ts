"use client";

import { useEffect, useState, useRef } from 'react';
import { useAuth } from './use-auth';
import { authApi } from '../lib/api/endpoints';
import { AUTH_KEYS, AUTH_EVENTS } from '../lib/constants';

// Session configuration constants
const TOKEN_LIFETIME_SECONDS = 15 * 60; // 15 minutes
const WARNING_THRESHOLD_SECONDS = 2 * 60; // 2 minutes (120 seconds warning)
const INACTIVITY_LIMIT_SECONDS = 13 * 60; // Inactivity lock at 13 minutes

export const useSessionTimeout = () => {
  const { isAuthenticated, logout, user } = useAuth();
  
  const [showWarning, setShowWarning] = useState(false);
  const [secondsRemaining, setSecondsRemaining] = useState(WARNING_THRESHOLD_SECONDS);
  
  const lastActivityTime = useRef<number>(Date.now());
  const countdownIntervalRef = useRef<NodeJS.Timeout | null>(null);
  const inactivityCheckRef = useRef<NodeJS.Timeout | null>(null);
  
  // Method to trigger silent refresh and extend session
  const extendSession = async () => {
    try {
      await authApi.refreshToken();
      
      // Reset state and hide warning modal
      setShowWarning(false);
      setSecondsRemaining(WARNING_THRESHOLD_SECONDS);
      lastActivityTime.current = Date.now();
      
      // Broadcast extension to other open tabs
      if (typeof window !== 'undefined') {
        const authChannel = new BroadcastChannel(AUTH_KEYS.BROADCAST_CHANNEL);
        authChannel.postMessage({
          type: AUTH_EVENTS.SESSION_EXTEND,
          payload: user,
        });
        authChannel.close();
      }
    } catch (error) {
      console.error('[Session Expiry Sync] Failed to silent-extend session:', error);
      logout(true);
    }
  };

  // Reset inactivity timer on human input
  const resetInactivityTimer = () => {
    lastActivityTime.current = Date.now();
    
    // If the modal was showing but we didn't officially trigger extend yet,
    // we can extend silently on action if still within valid token period!
    if (showWarning && secondsRemaining > 10) {
      extendSession();
    }
  };

  useEffect(() => {
    if (!isAuthenticated) {
      setShowWarning(false);
      return;
    }

    // Set up standard user input listeners to track active presence
    const activities = ['mousemove', 'keydown', 'mousedown', 'touchstart', 'scroll'];
    
    activities.forEach((activity) => {
      window.addEventListener(activity, resetInactivityTimer);
    });

    // 1. Periodic check for inactivity limits
    inactivityCheckRef.current = setInterval(() => {
      const inactiveDuration = (Date.now() - lastActivityTime.current) / 1000;
      
      // If inactive past threshold, trigger warning modal
      if (inactiveDuration >= INACTIVITY_LIMIT_SECONDS && !showWarning) {
        setShowWarning(true);
        setSecondsRemaining(WARNING_THRESHOLD_SECONDS);
      }
    }, 10000); // Check inactivity every 10 seconds

    // Listen for SESSION_EXTEND events from other tabs to sync timers locally
    const syncChannel = new BroadcastChannel(AUTH_KEYS.BROADCAST_CHANNEL);
    syncChannel.onmessage = (event) => {
      if (event.data.type === AUTH_EVENTS.SESSION_EXTEND) {
        setShowWarning(false);
        setSecondsRemaining(WARNING_THRESHOLD_SECONDS);
        lastActivityTime.current = Date.now();
      }
    };

    return () => {
      activities.forEach((activity) => {
        window.removeEventListener(activity, resetInactivityTimer);
      });
      if (inactivityCheckRef.current) clearInterval(inactivityCheckRef.current);
      syncChannel.close();
    };
  }, [isAuthenticated, showWarning]);

  // 2. Warning Modal Active Countdown
  useEffect(() => {
    if (!showWarning) {
      if (countdownIntervalRef.current) {
        clearInterval(countdownIntervalRef.current);
      }
      return;
    }

    countdownIntervalRef.current = setInterval(() => {
      setSecondsRemaining((prev) => {
        if (prev <= 1) {
          clearInterval(countdownIntervalRef.current!);
          logout(true); // Terminate session and redirect on zero
          return 0;
        }
        return prev - 1;
      });
    }, 1000); // Tick every 1 second

    return () => {
      if (countdownIntervalRef.current) clearInterval(countdownIntervalRef.current);
    };
  }, [showWarning, logout]);

  return {
    showWarning,
    secondsRemaining,
    extendSession,
    logout: () => logout(true),
  };
};
