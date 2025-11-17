// src/App.tsx
import { useEffect, useState } from 'react';
import './App.css';
import {
  getProducts,
  getCustomers,
  getInvoices,
  createCustomer,
  createInvoice,
} from './api';

type Product = { productID: number; name: string; productNumber: string; listPrice: number };
type Customer = { customerID: number; accountNumber: string; personID: number };
type Invoice = { salesOrderID: number; orderDate: string; subTotal: number; taxAmt: number; freight: number };

export default function App() {
  const [tab, setTab] = useState<'customers' | 'products' | 'invoices'>('customers');

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800 px-6 py-4">
        <h1 className="text-2xl font-semibold">AdventureWorks — Demo Frontend</h1>
        <p className="text-sm text-slate-400">API: http://localhost:5000</p>
      </header>

      <nav className="px-6 py-3 flex gap-2">
        {(['customers', 'products', 'invoices'] as const).map((k) => (
          <button
            key={k}
            onClick={() => setTab(k)}
            className={`px-3 py-2 rounded-xl border ${
              tab === k ? 'bg-slate-800 border-slate-700' : 'border-slate-800 hover:bg-slate-900'
            }`}
          >
            {k}
          </button>
        ))}
      </nav>

      <main className="p-6 grid gap-6">
        {tab === 'customers' && <CustomersView />}
        {tab === 'products' && <ProductsView />}
        {tab === 'invoices' && <InvoicesView />}
      </main>
    </div>
  );
}

function ProductsView() {
  const [data, setData] = useState<Product[]>([]);
  const [error, setError] = useState<string>('');

  const reload = () => getProducts().then(setData).catch((e) => setError(e.message));
  useEffect(() => {
    reload();
  }, []);

  return (
    <section className="grid gap-4">
      <h2 className="text-xl font-semibold">Produkty (TOP 100)</h2>
      {error && <p className="text-red-400">{error}</p>}
      <div className="overflow-auto rounded-xl border border-slate-800">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-900">
            <tr>
              <th className="text-left px-3 py-2">ID</th>
              <th className="text-left px-3 py-2">Name</th>
              <th className="text-left px-3 py-2">Number</th>
              <th className="text-right px-3 py-2">ListPrice</th>
            </tr>
          </thead>
          <tbody>
            {data.map((p) => (
              <tr key={p.productID} className="odd:bg-slate-900/40">
                <td className="px-3 py-2">{p.productID}</td>
                <td className="px-3 py-2">{p.name}</td>
                <td className="px-3 py-2">{p.productNumber}</td>
                <td className="px-3 py-2 text-right">{p.listPrice?.toFixed?.(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function CustomersView() {
  const [data, setData] = useState<Customer[]>([]);
  const [error, setError] = useState('');
  const [firstName, setFirst] = useState('Jan');
  const [lastName, setLast] = useState('Kowalski');
  const [territoryId, setTerritory] = useState<number | ''>('');

  const reload = () => getCustomers().then(setData).catch((e) => setError(e.message));
  useEffect(() => {
    reload();
  }, []);

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createCustomer({
        firstName,
        lastName,
        territoryId: territoryId === '' ? undefined : Number(territoryId),
      });
      setFirst('');
      setLast('');
      setTerritory('');
      reload();
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <section className="grid gap-4">
      <h2 className="text-xl font-semibold">Klienci (TOP 100) + dodawanie</h2>
      {error && <p className="text-red-400">{error}</p>}

      <form
        onSubmit={submit}
        className="grid md:grid-cols-4 gap-3 items-end bg-slate-900/40 p-4 rounded-xl border border-slate-800"
      >
        <div>
          <label className="block text-xs text-slate-400 mb-1">First name</label>
          <input
            value={firstName}
            onChange={(e) => setFirst(e.target.value)}
            className="w-full rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
          />
        </div>
        <div>
          <label className="block text-xs text-slate-400 mb-1">Last name</label>
          <input
            value={lastName}
            onChange={(e) => setLast(e.target.value)}
            className="w-full rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
          />
        </div>
        <div>
          <label className="block text-xs text-slate-400 mb-1">TerritoryId (opcjonalnie)</label>
          <input
            value={territoryId}
            onChange={(e) => setTerritory(e.target.value ? Number(e.target.value) : '')}
            type="number"
            className="w-full rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
          />
        </div>
        <button className="rounded-lg px-4 py-2 bg-emerald-600 hover:bg-emerald-500">Dodaj klienta</button>
      </form>

      <div className="overflow-auto rounded-xl border border-slate-800">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-900">
            <tr>
              <th className="text-left px-3 py-2">ID</th>
              <th className="text-left px-3 py-2">AccountNumber</th>
              <th className="text-left px-3 py-2">PersonID</th>
            </tr>
          </thead>
          <tbody>
            {data.map((c) => (
              <tr key={c.customerID} className="odd:bg-slate-900/40">
                <td className="px-3 py-2">{c.customerID}</td>
                <td className="px-3 py-2">{c.accountNumber}</td>
                <td className="px-3 py-2">{c.personID}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}

function InvoicesView() {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [error, setError] = useState('');
  const [customerId, setCustomerId] = useState<number>(1);
  const [issueDate, setIssueDate] = useState<string>(new Date().toISOString().slice(0, 10));
  const [lines, setLines] = useState([
    { productId: 707, quantity: 1, unitPrice: 100, unitPriceDiscount: 0 },
  ]);

  const reload = () => getInvoices().then(setInvoices).catch((e) => setError(e.message));
  useEffect(() => {
    reload();
  }, []);

  const updateLine = (i: number, patch: Partial<(typeof lines)[number]>) =>
    setLines((prev) => prev.map((l, idx) => (idx === i ? { ...l, ...patch } : l)));

  const addLine = () =>
    setLines((prev) => [...prev, { productId: 707, quantity: 1, unitPrice: 100, unitPriceDiscount: 0 }]);
  const removeLine = (i: number) => setLines((prev) => prev.filter((_, idx) => idx !== i));

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createInvoice({ customerId, issueDate, lines });
      setLines([{ productId: 707, quantity: 1, unitPrice: 100, unitPriceDiscount: 0 }]);
      reload();
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <section className="grid gap-4">
      <h2 className="text-xl font-semibold">Faktury (TOP 50) + tworzenie</h2>
      {error && <p className="text-red-400">{error}</p>}

      <form onSubmit={submit} className="grid gap-3 bg-slate-900/40 p-4 rounded-xl border border-slate-800">
        <div className="grid md:grid-cols-3 gap-3">
          <label className="grid gap-1">
            <span className="text-xs text-slate-400">Customer ID</span>
            <input
              type="number"
              value={customerId}
              onChange={(e) => setCustomerId(Number(e.target.value))}
              className="rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
            />
          </label>
          <label className="grid gap-1">
            <span className="text-xs text-slate-400">Issue date</span>
            <input
              type="date"
              value={issueDate}
              onChange={(e) => setIssueDate(e.target.value)}
              className="rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
            />
          </label>
        </div>

        <div className="grid gap-2">
          <div className="text-sm text-slate-400">Pozycje</div>
          {lines.map((l, i) => (
            <div key={i} className="grid md:grid-cols-5 gap-2 items-end">
              <input
                type="number"
                value={l.productId}
                onChange={(e) => updateLine(i, { productId: Number(e.target.value) })}
                placeholder="ProductID"
                className="rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
              />
              <input
                type="number"
                value={l.quantity}
                onChange={(e) => updateLine(i, { quantity: Number(e.target.value) })}
                placeholder="Qty"
                className="rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
              />
              <input
                type="number"
                step="0.01"
                value={l.unitPrice}
                onChange={(e) => updateLine(i, { unitPrice: Number(e.target.value) })}
                placeholder="UnitPrice"
                className="rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
              />
              <input
                type="number"
                step="0.01"
                value={l.unitPriceDiscount}
                onChange={(e) => updateLine(i, { unitPriceDiscount: Number(e.target.value) })}
                placeholder="Discount (0..1)"
                className="rounded-lg bg-slate-900 border border-slate-800 px-3 py-2"
              />
              <button
                type="button"
                onClick={() => removeLine(i)}
                className="px-3 py-2 rounded-lg border border-slate-700 hover:bg-slate-900"
              >
                Usuń
              </button>
            </div>
          ))}
          <button
            type="button"
            onClick={addLine}
            className="self-start px-3 py-2 rounded-lg border border-slate-700 hover:bg-slate-900"
          >
            + Dodaj pozycję
          </button>
        </div>

        <div>
          <button className="rounded-lg px-4 py-2 bg-indigo-600 hover:bg-indigo-500">Zapisz fakturę</button>
        </div>
      </form>

      <div className="overflow-auto rounded-xl border border-slate-800">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-900">
            <tr>
              <th className="text-left px-3 py-2">ID</th>
              <th className="text-left px-3 py-2">OrderDate</th>
              <th className="text-right px-3 py-2">SubTotal</th>
              <th className="text-right px-3 py-2">Tax</th>
              <th className="text-right px-3 py-2">Freight</th>
            </tr>
          </thead>
          <tbody>
            {invoices.map((inv) => (
              <tr key={inv.salesOrderID} className="odd:bg-slate-900/40">
                <td className="px-3 py-2">{inv.salesOrderID}</td>
                <td className="px-3 py-2">{new Date(inv.orderDate).toLocaleString()}</td>
                <td className="px-3 py-2 text-right">{inv.subTotal?.toFixed?.(2)}</td>
                <td className="px-3 py-2 text-right">{inv.taxAmt?.toFixed?.(2)}</td>
                <td className="px-3 py-2 text-right">{inv.freight?.toFixed?.(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
