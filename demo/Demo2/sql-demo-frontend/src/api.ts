export const API_BASE = 'http://localhost:5051';

async function api<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if (!res.ok) {
    const text = await res.text().catch(() => '');
    throw new Error(`${res.status} ${res.statusText} ${text}`.trim());
  }

  return res.json() as Promise<T>;
}

// ---------- Typy ----------

export type TelemetrySnapshot = {
  timestamp: string;
  activeSessions: number;
  activeRequests: number;
  queryStoreQueries: number;
  topQueryText: string;
};

export type AskResponseRow = Record<string, unknown>;

export type AskResponse = {
  question: string;
  sql: string;
  rows: AskResponseRow[];
};

// ---------- Funkcje API ----------

export const getTelemetrySnapshot = () =>
  api<TelemetrySnapshot>('/telemetry/snapshot');

export const askAssistant = (question: string) =>
  api<AskResponse>('/assistant/ask', {
    method: 'POST',
    body: JSON.stringify({ question }),
  });
