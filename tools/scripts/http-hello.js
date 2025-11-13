// tools/scripts/http-hello.js
// Minimalny serwer HTTP do Lab01 (port 3000)
// Funkcje: CORS (localhost/127.0.0.1), preflight, ETag, Cache-Control, redirect 301, nosniff

const http = require('http');
const crypto = require('crypto');

// ===== Konfiguracja =====
const PORT = process.env.PORT ? parseInt(process.env.PORT, 10) : 3000;
const ALLOWED_ORIGINS = new Set([
  'http://localhost:5500',
  'http://127.0.0.1:5500',
]);

// ETag demo (stały ładunek)
let etagPayload = 'Hello-ETag';
let etag = '"' + crypto.createHash('md5').update(etagPayload).digest('hex') + '"';

// Pomocnicze
function json(res, status, data, extraHeaders = {}) {
  res.writeHead(status, {
    'Content-Type': 'application/json; charset=utf-8',
    'X-Content-Type-Options': 'nosniff',
    ...extraHeaders,
  });
  res.end(JSON.stringify(data));
}

function text(res, status, body, extraHeaders = {}) {
  res.writeHead(status, {
    'Content-Type': 'text/plain; charset=utf-8',
    'X-Content-Type-Options': 'nosniff',
    ...extraHeaders,
  });
  res.end(body);
}

// Prosty logger
function log(req, res, start = Date.now()) {
  const duration = Date.now() - start;
  console.log(`${new Date().toISOString()} ${req.method} ${req.url} -> ${res.statusCode} ${duration}ms`);
}

const server = http.createServer(async (req, res) => {
  const start = Date.now();

  // --- CORS (dynamic origin + preflight) ---
  const origin = req.headers.origin;
  if (origin && ALLOWED_ORIGINS.has(origin)) {
    res.setHeader('Access-Control-Allow-Origin', origin);
    res.setHeader('Vary', 'Origin'); // ważne dla cache i CDN
  }
  res.setHeader('Access-Control-Allow-Credentials', 'true');
  res.setHeader('Access-Control-Allow-Headers', 'content-type');
  res.setHeader('Access-Control-Allow-Methods', 'GET,POST,OPTIONS');

  // Wspólne bezpieczeństwo
  res.setHeader('X-Content-Type-Options', 'nosniff');

  // Preflight (dla POST/niestandardowych nagłówków)
  if (req.method === 'OPTIONS') {
    res.writeHead(204);
    res.end();
    return log(req, res, start);
  }

  // Prosta obsługa body JSON dla POST (jeśli potrzebne w ćwiczeniach)
  let body = '';
  if (req.method === 'POST') {
    for await (const chunk of req) body += chunk;
  }

  try {
    // ===== Routing =====
    if (req.url === '/' && req.method === 'GET') {
      text(res, 200, 'OK — serwer działa');
      return log(req, res, start);
    }

    // Redirect 301 -> /
    if (req.url === '/old') {
      // Uwaga: nagłówki CORS już ustawione wyżej — dzięki temu redirect nie będzie blokowany przez przeglądarkę
      res.writeHead(301, { Location: '/' });
      res.end();
      return log(req, res, start);
    }

    // Hello (GET/POST)
    if (req.url === '/api/hello') {
      const method = req.method;
      let payloadInfo = '';
      if (method === 'POST') {
        const ct = req.headers['content-type'] || '';
        if (body && ct.includes('application/json')) {
          try {
            const parsed = JSON.parse(body);
            payloadInfo = `, body keys: ${Object.keys(parsed).join(', ') || '(none)'}`;
          } catch {
            payloadInfo = ', body: (invalid JSON)';
          }
        } else if (body) {
          payloadInfo = `, raw body length: ${Buffer.byteLength(body)}`;
        }
      }
      text(res, 200, `Hello over HTTP (${method}${payloadInfo})`);
      return log(req, res, start);
    }

    // Cache 60s
    if (req.url === '/api/cache60') {
      text(res, 200, 'Cache me for 60s', { 'Cache-Control': 'max-age=60' });
      return log(req, res, start);
    }

    // No-store
    if (req.url === '/api/nocache') {
      text(res, 200, 'No-store response', { 'Cache-Control': 'no-store' });
      return log(req, res, start);
    }

    // ETag demo (If-None-Match -> 304)
    if (req.url === '/api/etag') {
      const inm = req.headers['if-none-match'];
      if (inm && inm === etag) {
        res.writeHead(304, { ETag: etag });
        res.end();
        return log(req, res, start);
      }
      text(res, 200, etagPayload, { ETag: etag });
      return log(req, res, start);
    }

    // 404 JSON
    json(res, 404, { error: 'Not Found' });
    return log(req, res, start);
  } catch (err) {
    console.error('Unhandled error:', err);
    json(res, 500, { error: 'Internal Server Error' });
    return log(req, res, start);
  }
});

server.listen(PORT, () => {
  console.log(`HTTP server listening on http://localhost:${PORT}`);
});
