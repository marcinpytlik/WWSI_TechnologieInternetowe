import { useEffect, useState } from "react";
import "./App.css"; // możesz zostawić pusty albo skasować i usunąć import
import {
  getTelemetrySnapshot,
  askAssistant
} from "./api";
import type { TelemetrySnapshot, AskResponse } from "./api";


type Tab = "telemetry" | "assistant";

export default function App() {
  const [tab, setTab] = useState<Tab>("telemetry");

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 px-6 py-4 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold">
            SQL Demo – Telemetry &amp; Assistant
          </h1>
          <p className="text-xs text-slate-400">
            API: <span className="font-mono">http://localhost:5051</span>
          </p>
        </div>
        <span className="text-xs text-slate-500">
          WWSI – Technologie Internetowe · Demo 2
        </span>
      </header>

      <nav className="px-6 py-3 flex gap-2 border-b border-slate-800">
        <TabButton
          active={tab === "telemetry"}
          onClick={() => setTab("telemetry")}
        >
          Telemetry snapshot
        </TabButton>
        <TabButton
          active={tab === "assistant"}
          onClick={() => setTab("assistant")}
        >
          SQL assistant
        </TabButton>
      </nav>

      <main className="p-6 max-w-5xl mx-auto grid gap-6">
        {tab === "telemetry" && <TelemetryView />}
        {tab === "assistant" && <AssistantView />}
      </main>
    </div>
  );
}

function TabButton(props: {
  active: boolean;
  children: React.ReactNode;
  onClick: () => void;
}) {
  const { active, children, onClick } = props;
  return (
    <button
      onClick={onClick}
      className={`px-3 py-2 rounded-xl border text-sm transition
      ${
        active
          ? "bg-slate-800 border-slate-600"
          : "border-slate-800 hover:bg-slate-900"
      }`}
    >
      {children}
    </button>
  );
}

// ---------------- Telemetry ----------------

function TelemetryView() {
  const [snapshot, setSnapshot] = useState<TelemetrySnapshot | null>(null);
  const [error, setError] = useState<string>("");
  const [autoRefresh, setAutoRefresh] = useState<boolean>(true);
  const [intervalMs, setIntervalMs] = useState<number>(5000);

  const load = async () => {
    try {
      setError("");
      const s = await getTelemetrySnapshot();
      setSnapshot(s);
    } catch (e: any) {
      setError(e.message ?? String(e));
    }
  };

  // pierwszy load
  useEffect(() => {
    load();
  }, []);

  // auto-refresh z poprawnym cleanupem
  useEffect(() => {
    if (!autoRefresh) return;

    const id = window.setInterval(load, intervalMs);
    return () => window.clearInterval(id);
  }, [autoRefresh, intervalMs]);

  return (
    <section className="grid gap-4">
      <h2 className="text-xl font-semibold">Telemetry snapshot (live)</h2>

      <div className="flex items-center gap-3 text-xs text-slate-400">
        <button
          onClick={load}
          className="px-3 py-1 rounded-lg border border-slate-700 hover:bg-slate-900"
        >
          Odśwież teraz
        </button>

        <label className="inline-flex items-center gap-2">
          <input
            type="checkbox"
            checked={autoRefresh}
            onChange={(e) => setAutoRefresh(e.target.checked)}
          />
          Auto refresh
        </label>

        <label className="inline-flex items-center gap-2">
          co
          <input
            type="number"
            min={1000}
            step={1000}
            value={intervalMs}
            onChange={(e) => setIntervalMs(Number(e.target.value) || 1000)}
            className="w-20 px-2 py-1 rounded bg-slate-900 border border-slate-700 text-right"
          />
          ms
        </label>

        {snapshot && (
          <span>
            Last update:{" "}
            {new Date(snapshot.timestamp).toLocaleTimeString("pl-PL")}
          </span>
        )}
      </div>

      {error && (
        <p className="text-sm text-red-400 bg-red-950/40 border border-red-900 px-3 py-2 rounded-lg">
          {error}
        </p>
      )}

      {snapshot && (
        <>
          <div className="grid md:grid-cols-3 gap-4">
            <MetricCard
              label="Active sessions"
              value={snapshot.activeSessions}
            />
            <MetricCard
              label="Active requests"
              value={snapshot.activeRequests}
            />
            <MetricCard
              label="Query Store – rows"
              value={snapshot.queryStoreQueries}
            />
          </div>

          <div className="grid gap-2">
            <h3 className="text-sm font-semibold text-slate-300">
              Top CPU query (sys.dm_exec_query_stats)
            </h3>
            <pre className="text-xs bg-slate-900 border border-slate-800 rounded-lg p-3 overflow-auto max-h-64">
              {snapshot.topQueryText}
            </pre>
          </div>
        </>
      )}

      {!snapshot && !error && (
        <p className="text-sm text-slate-400">Ładowanie pierwszego snapshotu…</p>
      )}
    </section>
  );
}

function MetricCard(props: { label: string; value: number | string }) {
  return (
    <div className="rounded-xl border border-slate-800 bg-slate-900/60 px-4 py-3">
      <div className="text-xs text-slate-400">{props.label}</div>
      <div className="mt-1 text-2xl font-semibold">{props.value}</div>
    </div>
  );
}

// ---------------- Assistant ----------------

function AssistantView() {
  const [question, setQuestion] = useState<string>(
    "Pokaż klientów z Polski"
  );
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>("");
  const [response, setResponse] = useState<AskResponse | null>(null);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const res = await askAssistant(question);
      setResponse(res);
    } catch (err: any) {
      setError(err.message ?? String(err));
      setResponse(null);
    } finally {
      setLoading(false);
    }
  };

  // proste kolumny z pierwszego wiersza
  const columns =
    response && response.rows.length > 0
      ? Object.keys(response.rows[0])
      : [];

  return (
    <section className="grid gap-4">
      <h2 className="text-xl font-semibold">SQL Query Assistant (rule-based)</h2>

      <form
        onSubmit={submit}
        className="grid gap-3 bg-slate-900/40 p-4 rounded-xl border border-slate-800"
      >
        <label className="grid gap-1">
          <span className="text-xs text-slate-400">
            Pytanie po ludzku (po polsku / angielsku)
          </span>
          <textarea
            value={question}
            onChange={(e) => setQuestion(e.target.value)}
            rows={3}
            className="rounded-lg bg-slate-950 border border-slate-800 px-3 py-2 text-sm"
          />
        </label>

        <div className="flex gap-3 items-center">
          <button
            type="submit"
            disabled={loading || !question.trim()}
            className="px-4 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50"
          >
            {loading ? "Pytam SQL…" : "Wyślij do asystenta"}
          </button>
          <span className="text-xs text-slate-500">
            Przykłady: „Pokaż klientów z Polski”, „Top 10 najdroższych
            produktów”, „Sprzedaż wg kraju”
          </span>
        </div>
      </form>

      {error && (
        <p className="text-sm text-red-400 bg-red-950/40 border border-red-900 px-3 py-2 rounded-lg">
          {error}
        </p>
      )}

      {response && (
        <div className="grid gap-3">
          <div className="grid gap-1">
            <div className="text-xs text-slate-400">Wygenerowany SQL</div>
            <pre className="text-xs bg-slate-900 border border-slate-800 rounded-lg p-3 overflow-auto max-h-64">
{response.sql}
            </pre>
          </div>

          <div className="grid gap-1">
            <div className="text-xs text-slate-400">
              Wynik zapytania ({response.rows.length} wierszy)
            </div>

            {columns.length === 0 ? (
              <p className="text-sm text-slate-400">
                Brak wierszy do wyświetlenia.
              </p>
            ) : (
              <div className="overflow-auto border border-slate-800 rounded-lg max-h-80">
                <table className="min-w-full text-xs">
                  <thead className="bg-slate-900 sticky top-0">
                    <tr>
                      {columns.map((col) => (
                        <th
                          key={col}
                          className="px-3 py-2 text-left font-semibold"
                        >
                          {col}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {response.rows.map((row, idx) => (
                      <tr
                        key={idx}
                        className={idx % 2 === 0 ? "bg-slate-900/40" : ""}
                      >
                        {columns.map((col) => (
                          <td key={col} className="px-3 py-1">
                            {formatValue(row[col])}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </div>
      )}
    </section>
  );
}

function formatValue(v: unknown): string {
  if (v == null) return "";
  if (v instanceof Date) return v.toLocaleString("pl-PL");
  if (typeof v === "number") return v.toString();
  return String(v);
}
