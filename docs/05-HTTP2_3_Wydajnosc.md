# HTTP/2 i HTTP/3 — co zmieniają i jak to wykorzystać

## Dlaczego nowe wersje?
HTTP/1.1 ma problem z *head‑of‑line blocking* (blokowanie czoła kolejki) i dużą liczbą połączeń. HTTP/2 i HTTP/3 rozwiązują to inną warstwą transportową i sposobem pakowania danych.

---

## HTTP/2 (h2)
**Transport:** TCP + TLS (zwykle ALPN `h2`).  
**Kluczowe cechy:**
- **Multiplexing:** wiele żądań/odpowiedzi na *jednym* połączeniu (strumienie z priorytetami).
- **HPACK:** kompresja nagłówków (tablica dynamiczna + Huffman) zmniejsza narzut `Cookie`, `User-Agent`, itp.
- **Binary framing:** ramki zamiast tekstowej składni → mniej parowania i szybsze przetwarzanie.
- **Server push (historycznie):** serwer mógł „wypychać” zasoby zanim klient o nie poprosi. W praktyce wsparcie w przeglądarkach zostało wygaszone; dziś preferuje się **`Link: rel=preload`** i **`103 Early Hints`**.

**Praktyczne wnioski (front‑end):**
- **Nie** „szarduj” domen i **nie** otwieraj wielu połączeń tylko po to, by równoleglić – h2 robi to na jednym połączeniu.
- Sprite’y, sztuczne łączenie plików, data URI – to często **niepotrzebne**. Zamiast „jednego wielkiego bundle” preferuj **code‑splitting** + cache.
- Ustaw **priorytety zasobów** (preload/`as=style/script/font`) i sensowny cache (`Cache-Control`, `ETag`).

**Jak sprawdzić:**
```bash
curl -I --http2 https://example.com
# W nagłówkach: 'alt-svc: h3=...' lub 'server: ...' oraz 'HTTP/2 200'
```

---

## HTTP/3 (h3)
**Transport:** **QUIC** (UDP) + TLS 1.3, negocjacja przez ALPN `h3`.  
**Co poprawia względem h2/TCP:**
- **Brak blokowania czoła kolejki na poziomie transportu:** utrata pakietu nie stopuje wszystkich strumieni (jak w TCP), tylko dotknięty strumień.
- **Szybsze zestawianie połączeń:** 0‑RTT/1‑RTT dzięki TLS 1.3 i mechanizmom QUIC.
- **Migration:** połączenie może przetrwać zmianę IP/interfejsu (np. Wi‑Fi → LTE) dzięki identyfikatorom połączenia w QUIC.
- **Wbudowane szyfrowanie:** zawsze TLS 1.3, brak „plaintext H3”.

**Praktyczne wnioski:**
- Zyski widać głównie na **sieciach mobilnych** i przy utracie pakietów/opóźnieniach.
- Konfiguruj **HTTP/3 + Alt‑Svc** równolegle z HTTP/2 – klienci, które nie potrafią h3, zostaną na h2.
- Wciąż obowiązuje dobra **higiena cache** i **krytyczna ścieżka renderowania** (CSS!).

**Jak sprawdzić:**
```bash
curl -I --http3 https://example.com
# Szukaj 'HTTP/3 200' i/lub nagłówka 'alt-svc: h3=":443"; ma=...'
```

---

## Wzorce i anty‑wzorce w epoce h2/h3
**Tak:**
- `Link: <...css>; rel=preload; as=style` dla krytycznych styli, fontów i głównego bundla.
- Podział JS na krytyczny i leniwie dociągany (`dynamic import()`).
- Utrzymywanie **krótkich** i **stabilnych** nagłówków (kompresja pomaga, ale duże `Cookie` dalej bolą).

**Raczej nie:**
- Nadmierna liczba malutkich, *niecache’owanych* zasobów (nadal kosztuje).
- „Domain sharding” i ręczne sztuczki pod równoległość – h2/h3 robią to lepiej.
- Server Push (w nowych projektach) – preferuj **preload** albo **Early Hints 103**.

---

## Minimalna checklista konfiguracyjna
1. **TLS nowoczesny:** włącz ALPN, TLS 1.3, sensowne pakiety szyfrów; certyfikaty automatyczne (np. ACME/Let’s Encrypt).
2. **h2 + h3 równolegle:** serwer/edge (CDN) powinien wystawiać oba; dodaj `alt-svc` dla h3.
3. **Cache:** `Cache-Control: public, max-age=...`, unikalne nazwy (hash w ścieżce) dla długiego cache statyków.
4. **Preload:** krytyczne CSS/JS/fonty z poprawnym `as=` i CORS (dla fontów `crossorigin`).
5. **Rozsądne rozmiary:** kompresja (gzip/br) i HTTP/2 prioritization nie zastąpią optymalizacji obrazu.

---

## Narzędzia do testów
- **Przeglądarka → DevTools → Network:** protokół kolumna „Protocol” (np. `h2`, `h3`).  
- **`curl`**: `--http2` / `--http3` + `-I`.  
- **Lighthouse/WebPageTest:** metryki czasu (TTFB, LCP), waterfall (sprawdź, czy nie masz długiego CSS‑blocking).

---

## FAQ (skrót)
- **Czy muszę migrować wszystkie optymalizacje h1.1?** Nie. Zacznij od porządnego cache i preloading. Bundlowanie „na siłę” bywa przeciwproduktywne.
- **Co z HTTP/2 Server Push?** W praktyce jest martwy po stronie przeglądarek. Używaj `preload` i **103 Early Hints** (jeśli serwer/CDN wspiera).
- **Czy QUIC otwiera firewallowe problemy?** Sporadycznie UDP bywa blokowane. Dlatego trzymaj **fallback na h2/TCP**.
