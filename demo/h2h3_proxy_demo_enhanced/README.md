# NGINX HTTP/2 + HTTP/3 (QUIC) — demo do wykładu (z „waterfallem”)

Ten zestaw stawia reverse proxy z TLS, H2 i H3 *oraz* serwuje lokalnie stronę `index.html` z wieloma zasobami (`/assets/*`), żeby ładnie pokazać multiplexing w DevTools.

## Wymagania
- Docker Desktop
- porty 80/443 wolne
- narzędzie do wystawienia certu dla `localhost` (polecam **mkcert**)

## 1) Wygeneruj certy
```bash
# jednorazowo
mkcert -install
# w katalogu projektu
mkcert -cert-file certs/localhost.crt -key-file certs/localhost.key localhost 127.0.0.1 ::1
```

*(Alternatywnie OpenSSL — ale przeglądarka będzie ostrzegać.)*

## 2) (Opcjonalnie) uruchom backend
Jeśli chcesz też proxy do API (np. LivePoll na :5171):
- Windows/macOS: działa od razu (używamy `host.docker.internal`)
- Linux: w `nginx.conf` podmień upstream na IP hosta (`172.17.0.1` albo własne)

## 3) Start NGINX
```bash
docker compose up -d
```

## 4) Pokaż na żywo
- wejdź na **https://localhost**
- DevTools → Network → kolumna **Protocol** (h2/h3)
- kliknij „Załaduj moduły” i „Załaduj obrazy”; patrz na waterfall i równoległość

## 5) Testy curl
```bash
curl -I --http2 https://localhost
curl -I --http3 https://localhost  # jeśli curl ma wsparcie H3/QUIC
```

## Struktura
- `docker-compose.yml` — uruchamia NGINX (tcp/443 + udp/443)
- `nginx.conf` — TLS 1.3, `http2` + `quic`, `Alt-Svc`, `/assets` lokalnie, `/api` do backendu
- `index.html` — strona z przyciskami/assetami
- `assets/` — 6 CSS, 12 modułów JS, 20 SVG (dla ładnego wodospadu)
- `certs/` — wstaw tu `localhost.crt` i `localhost.key`

## Pro tipy do zajęć
- włącz „Disable cache” w DevTools
- pokaż nagłówek `Alt-Svc` i listę protokołów w Network
- porównaj z tymczasowym serwerem na HTTP/1.1 (bez h2/h3) i pokaż różnicę w waterfallu

## 6) Tryb porównawczy — HTTP/1.1 vs H2/H3
Chcesz pokazać różnicę w „waterfallu”? Odpal obok wersję **HTTP/1.1 only** na innych portach.

### Start H1.1
```bash
docker compose -f docker-compose.h1.yml up -d
```

- **HTTP/1.1:** https://localhost:8443 (albo http://localhost:8080)  
- **H2/H3:**    https://localhost

### Co porównać w DevTools
- kolumna **Protocol** (`http/1.1` vs `h2`/`h3`)
- równoległość pobierania kilkunastu plików (`/assets/*`)
- czasy TTFB/Content Download przy „Disable cache”

### curl
```bash
# H1.1 (bez http2)
curl -I https://localhost:8443

# H2/H3
curl -I --http2 https://localhost
curl -I --http3 https://localhost
```
