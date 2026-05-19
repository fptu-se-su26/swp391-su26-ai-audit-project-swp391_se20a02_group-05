"use client";

import React, { useState, useEffect, useCallback } from 'react';
import { adminService } from '../../../../services/admin.service';
import { AuditLogListItem } from '../../../../types/admin.types';
import {
  Table,
  Button,
  Card
} from '@heroui/react';
import { Search, RotateCw, ShieldCheck } from 'lucide-react';
import { PaginationWrapper } from '../../../../components/ui/pagination-wrapper';
import { SkeletonLoader, EmptyState } from '../../../../components/ui/states';
import { useTranslation } from 'react-i18next';

export default function AuditLogsPage() {
  const { t } = useTranslation(['dashboard-admin', 'common']);
  const [logs, setLogs] = useState<AuditLogListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(15);
  const [search, setSearch] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [isRefreshing, setIsRefreshing] = useState(false);

  // Debounced search logic to prevent excessive backend queries
  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedSearch(search);
    }, 300);

    return () => {
      clearTimeout(handler);
    };
  }, [search]);

  const fetchLogs = useCallback(async (currentPage: number, searchString: string, silent = false) => {
    if (!silent) setIsLoading(true);
    try {
      const response = await adminService.getAuditLogs({
        search: searchString || undefined,
        page: currentPage,
        pageSize
      });
      setLogs(response.items);
      setTotalCount(response.totalCount);
    } catch (err) {
      console.error('Failed to fetch audit logs', err);
    } finally {
      setIsLoading(false);
      setIsRefreshing(false);
    }
  }, [pageSize]);

  useEffect(() => {
    const timer = setTimeout(() => {
      fetchLogs(page, debouncedSearch);
    }, 0);
    return () => clearTimeout(timer);
  }, [page, debouncedSearch, fetchLogs]);

  const handleRefresh = () => {
    setIsRefreshing(true);
    fetchLogs(page, debouncedSearch, true);
  };

  const totalPages = Math.ceil(totalCount / pageSize) || 1;

  const getEventTypeStyle = (type: string) => {
    const t = type.toUpperCase();
    if (t.includes('FAIL') || t.includes('REVOKE') || t.includes('SUSPEND') || t.includes('BAN')) {
      return { color: 'danger' as const, label: type };
    }
    if (t.includes('CREATE') || t.includes('SUCCESS') || t.includes('SYNC') || t.includes('LOGIN')) {
      return { color: 'success' as const, label: type };
    }
    if (t.includes('UPDATE') || t.includes('EDIT')) {
      return { color: 'warning' as const, label: type };
    }
    return { color: 'primary' as const, label: type };
  };

  return (
    <div className="space-y-6 font-outfit max-w-7xl mx-auto p-4 md:p-6 text-zinc-900 dark:text-zinc-550">
      {/* Title block */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-extrabold tracking-tight text-zinc-900 dark:text-zinc-550 flex items-center gap-2">
            <ShieldCheck className="text-emerald-500" size={24} />
            {t('dashboard-admin:auditLogs.title')}
          </h1>
          <p className="text-zinc-500 dark:text-zinc-400 text-sm">
            {t('dashboard-admin:auditLogs.subtitle')}
          </p>
        </div>
        <Button
          variant="secondary"
          onPress={handleRefresh}
          className="w-fit px-4 py-2.5 bg-zinc-900 text-white dark:bg-zinc-100 dark:text-zinc-950 font-bold rounded-xl text-xs flex items-center gap-2 hover:opacity-90 transition-opacity select-none cursor-pointer"
        >
          <RotateCw size={14} className={isRefreshing ? 'animate-spin' : ''} />
          {t('dashboard-admin:auditLogs.syncRecords')}
        </Button>
      </div>

      {/* Search Filter Banner */}
      <Card className="p-4 bg-white/70 dark:bg-zinc-950/60 backdrop-blur-xl border border-zinc-200/50 dark:border-zinc-900 rounded-2xl shadow-sm">
        <div className="flex-1 relative">
          <Search size={16} className="absolute left-3 top-3.5 text-zinc-400" />
          <input
            type="text"
            placeholder={t('dashboard-admin:auditLogs.searchPlaceholder')}
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-10 pr-4 py-2.5 rounded-xl border border-zinc-200/60 dark:border-zinc-800 bg-white/50 dark:bg-zinc-900/50 text-xs focus:outline-none focus:ring-2 focus:ring-indigo-500/20"
          />
        </div>
      </Card>

      {/* Audit Log Table */}
      <Card className="p-0 overflow-hidden border border-zinc-200/50 dark:border-zinc-900 bg-white/80 dark:bg-zinc-950/70 backdrop-blur-xl rounded-2xl shadow-sm">
        {isLoading ? (
          <SkeletonLoader rows={6} columns={5} />
        ) : logs.length === 0 ? (
          <EmptyState
            title={t('dashboard-admin:auditLogs.empty.title')}
            description={t('dashboard-admin:auditLogs.empty.description')}
          />
        ) : (
          <div className="overflow-x-auto">
            <Table aria-label={t('dashboard-admin:auditLogs.table.ariaLabel')} className="w-full">
              <Table.ScrollContainer>
                <Table.Content aria-label={t('dashboard-admin:auditLogs.table.ariaLabelContent')}>
                  <Table.Header>
                    <Table.Column isRowHeader className="font-extrabold uppercase text-[10px] tracking-wider py-4">{t('dashboard-admin:auditLogs.table.timestamp')}</Table.Column>
                    <Table.Column className="font-extrabold uppercase text-[10px] tracking-wider py-4">{t('dashboard-admin:auditLogs.table.triggeredBy')}</Table.Column>
                    <Table.Column className="font-extrabold uppercase text-[10px] tracking-wider py-4">{t('dashboard-admin:auditLogs.table.eventType')}</Table.Column>
                    <Table.Column className="font-extrabold uppercase text-[10px] tracking-wider py-4">{t('dashboard-admin:auditLogs.table.actionDetail')}</Table.Column>
                    <Table.Column className="font-extrabold uppercase text-[10px] tracking-wider py-4 hidden md:table-cell">{t('dashboard-admin:auditLogs.table.connectionOrigin')}</Table.Column>
                  </Table.Header>
                  <Table.Body>
                    {logs.map((log) => {
                      const badge = getEventTypeStyle(log.eventType);
                      return (
                        <Table.Row key={log.id} className="border-b border-zinc-100 dark:border-zinc-900/60 last:border-none hover:bg-zinc-50/40 dark:hover:bg-zinc-900/20">
                          <Table.Cell className="text-zinc-500 font-mono text-[11px] py-4 whitespace-nowrap">
                            {new Date(log.createdAt).toLocaleString()}
                          </Table.Cell>
                          <Table.Cell className="font-bold text-zinc-800 dark:text-zinc-200 text-xs py-4">
                            {log.userEmail || <span className="text-zinc-400 dark:text-zinc-600 font-normal">{t('dashboard-admin:auditLogs.systemContext')}</span>}
                          </Table.Cell>
                          <Table.Cell className="py-4">
                            <span className={`inline-flex items-center px-2 py-0.5 rounded text-[10px] font-extrabold tracking-wide uppercase ${
                              badge.color === 'danger' ? 'bg-red-50 text-red-700 dark:bg-red-950/40 dark:text-red-300 border border-red-200/10' :
                              badge.color === 'success' ? 'bg-emerald-50 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300 border border-emerald-200/10' :
                              badge.color === 'warning' ? 'bg-amber-50 text-amber-700 dark:bg-amber-950/40 dark:text-amber-300 border border-amber-200/10' :
                              'bg-indigo-50 text-indigo-700 dark:bg-indigo-950/40 dark:text-indigo-300 border border-indigo-200/10'
                            }`}>
                              {badge.label}
                            </span>
                          </Table.Cell>
                          <Table.Cell className="text-zinc-600 dark:text-zinc-350 text-xs max-w-md py-4 leading-relaxed font-normal">
                            {log.description}
                          </Table.Cell>
                          <Table.Cell className="py-4 whitespace-nowrap hidden md:table-cell">
                            <div className="flex flex-col text-[10px] text-zinc-450 dark:text-zinc-500 font-mono">
                              <span>IP: {log.ipAddress || 'Internal'}</span>
                              <span className="max-w-[150px] truncate" title={log.userAgent || 'None'}>
                                UA: {log.userAgent || 'None'}
                              </span>
                            </div>
                          </Table.Cell>
                        </Table.Row>
                      );
                    })}
                  </Table.Body>
                </Table.Content>
              </Table.ScrollContainer>
            </Table>
          </div>
        )}

        {logs.length > 0 && (
          <PaginationWrapper
            page={page}
            totalPages={totalPages}
            totalItems={totalCount}
            itemsPerPage={pageSize}
            onPageChange={(p) => setPage(p)}
          />
        )}
      </Card>
    </div>
  );
}
