// src/api.ts

export const API_BASE = 'http://localhost:5037';

async function api<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if (!res.ok) {
    const text = await res.text().catch(() => '');
    throw new Error(`${res.status} ${res.statusText} ${text}`.trim());
  }

  return res.json() as Promise<T>;
}

// ================== Typy ==================

export type WaitRow = {
  wait_type: string;
  wait_time_ms: number;
  signal_wait_time_ms?: number;
  resource_wait_ms?: number;
  pct: number;
};

export type TopCpuQuery = {
  db_name: string | null;
  object_name: string | null;
  execution_count: number;
  cpu_ms: number;
  duration_ms: number;
  query_text: string;
};

export type DashboardResponse = {
  timestamp: string;
  activeSessions: number;
  activeRequests: number;
  waits: WaitRow[];
  topCpuQueries: TopCpuQuery[];
  tempdb: Record<string, unknown>;
  log: Record<string, unknown>;
  queryStore: {
    isOn: boolean;
    totalQueries: number;
  };
};

export type PulseResponse = {
  timestamp: string;
  activeSessions: number;
  activeRequests: number;
  waits: {
    wait_type: string;
    wait_time_ms: number;
    pct: number;
  }[];
};

export type AdvisorResponse = {
  timestamp: string;
  waits: WaitRow[];
  tempdb: Record<string, unknown>;
  log: Record<string, unknown>;
  severity: string;
  messages: string[];
};

export type QueryExecResponse = {
  sql: string;
  rowCount: number;
  rows: Record<string, unknown>[];
};

// ================== Funkcje ==================

export const getDashboard = () => api<DashboardResponse>('/telemetry/dashboard');
export const getPulse = () => api<PulseResponse>('/telemetry/pulse');
export const getAdvisor = () => api<AdvisorResponse>('/telemetry/advisor');

export const execQuery = (sql: string) =>
  api<QueryExecResponse>('/telemetry/query-exec', {
    method: 'POST',
    body: JSON.stringify({ sql }),
  });
