// src/api.ts
export const API_BASE = 'http://localhost:5000';

export type Product = { productID: number; name: string; productNumber: string; listPrice: number; };
export type Customer = { customerID: number; accountNumber: string; personID: number; };
export type Invoice  = { salesOrderID: number; orderDate: string; subTotal: number; taxAmt: number; freight: number; };

function httpError(res: Response, body: string) {
  const msg = body || `${res.status} ${res.statusText}`;
  const err = new Error(msg);
  (err as any).status = res.status;
  return err;
}

async function api<T>(url: string, options?: RequestInit): Promise<T> {
  const ac = new AbortController();
  const t = setTimeout(() => ac.abort(), 15_000); // 15s timeout

  try {
    const res = await fetch(`${API_BASE}${url}`, {
      headers: { 'Content-Type': 'application/json' },
      signal: ac.signal,
      ...options,
    });
    const text = await res.text(); // wczytaj raz
    if (!res.ok) throw httpError(res, text);
    return (text ? JSON.parse(text) : null) as T;
  } finally {
    clearTimeout(t);
  }
}

export const getProducts = () => api<Product[]>('/aw/products');
export const getCustomers = () => api<Customer[]>('/aw/customers');
export const getInvoices = () => api<Invoice[]>('/aw/invoices');

export const createCustomer = (payload: {
  firstName: string; lastName: string; territoryId?: number;
}) => api<{ customerID: number }>(
  '/aw/customers',
  { method: 'POST', body: JSON.stringify(payload) }
);

export const createInvoice = (payload: {
  customerId: number;
  issueDate: string; // YYYY-MM-DD
  lines: { productId: number; quantity: number; unitPrice: number; unitPriceDiscount: number }[];
}) => api<{ salesOrderID: number }>(
  '/aw/invoices',
  { method: 'POST', body: JSON.stringify(payload) }
);
