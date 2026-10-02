# Certyfikat laboratoryjny

Wygeneruj lokalny certyfikat:

```powershell
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -subj "/CN=localhost"
```

Pliki certyfikatu nie są commitowane do repo.
