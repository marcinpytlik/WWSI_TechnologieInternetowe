import { useEffect, useState } from 'react';
import { getDashboard, getPulse, getAdvisor, execQuery } from './api';
import type {
  DashboardResponse,
  PulseResponse,
  AdvisorResponse,
  QueryExecResponse,
} from './api';

type Tab = 'dashboard' | 'advisor' | 'query';

export default function App() {
  const [tab, setTab] = useState<Tab>('dashboard');

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 px-6 py-4 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">
            SQLManiak – Telemetry PRO
          </h1>
          <p className="text-xs text-slate-400">
            Backend: <span className="font-mono">http://localhost:5051</span> / AdventureWorks2022
          </p>
        </div>
        <div className="text-right text-xs text-slate-400">
          <div>Demo: live DMVs + advisor</div>
          <div>React + TypeScript + Tailwind</div>
        </div>
      </header>

      <nav className="px-6 py-3 flex gap-2 border-b border-slate-900">
        {[
          { key: 'dashboard', label: 'Dashboard' },
          { key: 'advisor', label: 'Advisor' },
          { key: 'query', label: 'Query Explorer' },
        ].map((t) => (
          <button
            key={t.key}
            onClick={() => setTab(t.key as Tab)}
            className={`px-3 py-2 rounded-xl text-sm border transition ${
              tab === t.key
                ? 'bg-slate-800 border-slate-600'
                : 'border-slate-800 hover:bg-slate-900'
            }`}
          >
            {t.label}
          </button>
        ))}
      </nav>

      <main className="p-6 max-w-6xl mx-auto grid gap-6">
        {tab === 'dashboard' && <DashboardView />}
        {tab === 'advisor' && <AdvisorView />}
        {tab === 'query' && <QueryExplorerView />}
      </main>
    </div>
  );
}

// =================== DASHBOARD ===================

function DashboardView() {
  const [data, setData] = useState<DashboardResponse | null>(null);
  const [pulse, setPulse] = useState<PulseResponse | null>(null);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    // initial load
    getDashboard().then(setData).catch((e) => setError(e.message));
  }, []);

  useEffect(() => {
    const interval = setInterval(() => {
      getPulse()
        .then(setPulse)
        .catch((e) => setError(e.message));
    }, 3000);

    return () => clearInterval(interval);
  }, []);

  const activeSessions = pulse?.activeSessions ?? data?.activeSessions ?? 0;
  const activeRequests = pulse?.activeRequests ?? data?.activeRequests ?? 0;
  const waits = pulse?.waits ?? data?.waits ?? [];

  return (
    <section className="grid gap-4">
      <div className="flex items-center justify-between gap-4">
        <h2 className="text-xl font-semibold">Dashboard – Live Telemetry</h2>
        <div className="text-xs text-slate-400">
          Snapshot: {data?.timestamp && formatTime(data.timestamp)} | Pulse:{' '}
          {pulse?.timestamp && formatTime(pulse.timestamp)}
        </div>
      </div>

      {error && (
        <p className="text-sm text-red-400 bg-red-950/40 border border-red-900 px-3 py-2 rounded-lg">
          {error}
        </p>
      )}

      <div className="grid md:grid-cols-4 gap-3">
        <MetricCard label="Active sessions" value={activeSessions} />
        <MetricCard label="Active requests" value={activeRequests} />
        <MetricCard
          label="Query Store"
          value={
            data?.queryStore.isOn
              ? `${data.queryStore.totalQueries} queries`
              : 'OFF'
          }
        />
        <MetricCard
          label="Tempdb total MB"
          value={Number(data?.tempdb?.['total_mb'] ?? 0).toFixed(0)}
        />
      </div>

      <div className="grid md:grid-cols-2 gap-4">
        <Card title="Top waits">
          <table className="w-full text-xs">
            <thead className="bg-slate-900">
              <tr>
                <th className="text-left px-3 py-2">Wait type</th>
                <th className="text-right px-3 py-2">Wait ms</th>
                <th className="text-right px-3 py-2">%</th>
              </tr>
            </thead>
            <tbody>
              {waits.map((w, i) => (
                <tr key={i} className="odd:bg-slate-900/40">
                  <td className="px-3 py-2">{w.wait_type}</td>
                  <td className="px-3 py-2 text-right">
                    {Number(w.wait_time_ms).toLocaleString('pl-PL')}
                  </td>
                  <td className="px-3 py-2 text-right">{w.pct.toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </Card>

        <Card title="Top CPU queries">
          <div className="max-h-72 overflow-auto text-xs">
            {data?.topCpuQueries.map((q, i) => (
              <div
                key={i}
                className="border-b border-slate-800 last:border-b-0 px-3 py-2 grid gap-1"
              >
                <div className="flex justify-between gap-3">
                  <div className="font-mono text-[11px] text-emerald-300">
                    {q.db_name ?? '(db)'} · {q.object_name ?? '(stmt)'}
                  </div>
                  <div className="text-right text-[11px] text-slate-400">
                    CPU: {q.cpu_ms} ms · Exec: {q.execution_count}
                  </div>
                </div>
                <pre className="whitespace-pre-wrap text-[11px] text-slate-200">
                  {q.query_text}
                </pre>
              </div>
            ))}
            {!data?.topCpuQueries?.length && (
              <p className="px-3 py-2 text-slate-500 text-xs">
                Brak danych o top CPU – instancja może być idle.
              </p>
            )}
          </div>
        </Card>
      </div>

      <div className="grid md:grid-cols-2 gap-4">
        <Card title="Log stats">
          <pre className="text-xs text-slate-300 bg-slate-900/60 rounded-lg p-3 overflow-auto">
            {JSON.stringify(data?.log ?? {}, null, 2)}
          </pre>
        </Card>
        <Card title="Tempdb details">
          <pre className="text-xs text-slate-300 bg-slate-900/60 rounded-lg p-3 overflow-auto">
            {JSON.stringify(data?.tempdb ?? {}, null, 2)}
          </pre>
        </Card>
      </div>
    </section>
  );
}

function MetricCard(props: { label: string; value: string | number }) {
  return (
    <div className="rounded-xl border border-slate-800 bg-slate-900/60 px-4 py-3">
      <div className="text-xs text-slate-400">{props.label}</div>
      <div className="mt-1 text-xl font-semibold">
        {typeof props.value === 'number'
          ? props.value.toLocaleString('pl-PL')
          : props.value}
      </div>
    </div>
  );
}

function Card(props: { title: string; children: React.ReactNode }) {
  return (
    <section className="rounded-xl border border-slate-800 bg-slate-900/60">
      <header className="px-4 py-2 border-b border-slate-800 flex items-center justify-between">
        <h3 className="text-sm font-semibold">{props.title}</h3>
      </header>
      <div className="p-3">{props.children}</div>
    </section>
  );
}

function formatTime(iso: string) {
  const d = new Date(iso);
  return d.toLocaleTimeString('pl-PL');
}

// =================== ADVISOR ===================

function AdvisorView() {
  const [data, setData] = useState<AdvisorResponse | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const load = async () => {
    try {
      setLoading(true);
      const res = await getAdvisor();
      setData(res);
      setError('');
    } catch (e: any) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const severityColor =
    data?.severity === 'Critical'
      ? 'text-red-400'
      : data?.severity === 'Warning'
      ? 'text-yellow-300'
      : 'text-emerald-300';

  return (
    <section className="grid gap-4">
      <div className="flex justify-between items-center gap-4">
        <div>
          <h2 className="text-xl font-semibold">Advisor – SQLManiak rules</h2>
          <p className="text-xs text-slate-400">
            Proste reguły oparte o waits + tempdb + log
          </p>
        </div>
        <button
          onClick={load}
          disabled={loading}
          className="px-3 py-2 rounded-lg border border-slate-700 text-sm hover:bg-slate-900 disabled:opacity-50"
        >
          {loading ? 'Odświeżanie…' : 'Odśwież teraz'}
        </button>
      </div>

      {error && (
        <p className="text-sm text-red-400 bg-red-950/40 border border-red-900 px-3 py-2 rounded-lg">
          {error}
        </p>
      )}

      <div className="grid md:grid-cols-3 gap-4">
        <Card title="Severity">
          <div className={`text-lg font-semibold ${severityColor}`}>
            {data?.severity ?? '–'}
          </div>
          <div className="text-xs text-slate-400 mt-1">
            {data?.timestamp && `Snapshot: ${formatTime(data.timestamp)}`}
          </div>
        </Card>
        <Card title="Tempdb (summary)">
          <pre className="text-xs text-slate-300 bg-slate-900/60 rounded-lg p-3 overflow-auto">
            {JSON.stringify(data?.tempdb ?? {}, null, 2)}
          </pre>
        </Card>
        <Card title="Log (summary)">
          <pre className="text-xs text-slate-300 bg-slate-900/60 rounded-lg p-3 overflow-auto">
            {JSON.stringify(data?.log ?? {}, null, 2)}
          </pre>
        </Card>
      </div>

      <Card title="Messages">
        <ul className="text-sm text-slate-200 list-disc pl-6 space-y-1">
          {data?.messages?.map((m, i) => (
            <li key={i}>{m}</li>
          )) ?? <li>Brak danych z advisora.</li>}
        </ul>
      </Card>

      <Card title="Waits used for rules">
        <table className="w-full text-xs">
          <thead className="bg-slate-900">
            <tr>
              <th className="text-left px-3 py-2">Wait type</th>
              <th className="text-right px-3 py-2">Wait ms</th>
              <th className="text-right px-3 py-2">%</th>
            </tr>
          </thead>
          <tbody>
            {data?.waits?.map((w, i) => (
              <tr key={i} className="odd:bg-slate-900/40">
                <td className="px-3 py-2">{w.wait_type}</td>
                <td className="px-3 py-2 text-right">
                  {Number(w.wait_time_ms).toLocaleString('pl-PL')}
                </td>
                <td className="px-3 py-2 text-right">
                  {Number(w.pct).toFixed(2)}
                </td>
              </tr>
            )) ?? (
              <tr>
                <td className="px-3 py-2 text-slate-500" colSpan={3}>
                  Brak danych.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </Card>
    </section>
  );
}

// =================== QUERY EXPLORER ===================

function QueryExplorerView() {
  const [sql, setSql] = useState<string>(
    "SELECT TOP (10) ProductID, Name, ListPrice\nFROM Production.Product\nORDER BY ListPrice DESC;"
  );
  const [result, setResult] = useState<QueryExecResponse | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const run = async () => {
    setError('');
    setResult(null);
    try {
      setLoading(true);
      const res = await execQuery(sql);
      setResult(res);
    } catch (e: any) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  };

  const columns =
    result?.rows?.length && Object.keys(result.rows[0] as Record<string, unknown>);

  return (
    <section className="grid gap-4">
      <h2 className="text-xl font-semibold">Query Explorer (SELECT only)</h2>

      <div className="grid md:grid-cols-[2fr,3fr] gap-4">
        <div className="grid gap-3">
          <label className="grid gap-1 text-sm">
            <span className="text-xs text-slate-400">T-SQL (tylko SELECT)</span>
            <textarea
              value={sql}
              onChange={(e) => setSql(e.target.value)}
              className="min-h-[200px] rounded-lg bg-slate-900 border border-slate-800 px-3 py-2 font-mono text-xs"
            />
          </label>
          <div className="flex gap-2">
            <button
              onClick={run}
              disabled={loading}
              className="px-4 py-2 rounded-lg bg-emerald-600 hover:bg-emerald-500 text-sm disabled:opacity-50"
            >
              {loading ? 'Executing…' : 'Run'}
            </button>
          </div>
          {error && (
            <p className="text-sm text-red-400 bg-red-950/40 border border-red-900 px-3 py-2 rounded-lg">
              {error}
            </p>
          )}
        </div>

        <div className="grid gap-2">
          <div className="flex justify-between items-center text-xs text-slate-400">
            <div>Rows: {result?.rowCount ?? 0}</div>
            <div className="font-mono truncate max-w-[60%]">
              {result?.sql ?? ''}
            </div>
          </div>

          <div className="border border-slate-800 rounded-lg overflow-auto max-h-[400px]">
            {result && columns && columns.length > 0 ? (
              <table className="min-w-full text-xs">
                <thead className="bg-slate-900">
                  <tr>
                    {columns.map((c) => (
                      <th key={c} className="text-left px-3 py-2 font-medium">
                        {c}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {result.rows.map((row, i) => (
                    <tr key={i} className="odd:bg-slate-900/40">
                      {columns.map((c) => (
                        <td key={c} className="px-3 py-1">
                          {formatCell((row as any)[c])}
                        </td>
                      ))}
                    </tr>
                  ))}
                </tbody>
              </table>
            ) : (
              <div className="text-xs text-slate-500 px-3 py-2">
                Brak wyników – uruchom zapytanie.
              </div>
            )}
          </div>
        </div>
      </div>
    </section>
  );
}

function formatCell(v: unknown): string {
  if (v == null) return 'NULL';
  if (typeof v === 'number') return v.toString();
  if (v instanceof Date) return v.toISOString();
  return String(v);
}
