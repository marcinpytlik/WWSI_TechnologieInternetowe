# Certyfikat laboratoryjny

Wygeneruj lokalny certyfikat i klucz:

```powershell
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout localhost.key -out localhost.crt -subj "/CN=localhost"
```

Pliki:

```text
localhost.crt
localhost.key
```

pozostają lokalnie i nie są commitowane do repo.
