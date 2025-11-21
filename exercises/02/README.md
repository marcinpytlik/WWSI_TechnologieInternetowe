# Ćwiczenia – CSS layouty i frameworki

## Co tu jest
- **index.html** – układ strony zbudowany przy użyciu **Flexbox**: nagłówek, menu boczne, treść i stopka.
- **css/styles.css** – styl z **media queries** (<= 768px: układ mobilny – menu pod treścią).
- **bootstrap.html** *(opcjonalnie)* – formularz z użyciem siatki **Bootstrap 5** (CDN).

## Jak testować
1. Otwórz `index.html`. Zmniejsz szerokość okna do ~768px i zobacz, jak menu spada pod treść.
2. Otwórz `bootstrap.html`. Formularz wykorzystuje klasę `row g-3` i kolumny `col-*` do responsywnego układu.

## Zadania dla studentów
- Zmień szerokość sidebaru i obserwuj wpływ na layout.
- Dodaj dodatkowy breakpoint, np. `@media (max-width: 1024px)` i przestaw siatkę `.cards`.
- W formularzu Bootstrap dodaj nową sekcję i przetestuj różne kombinacje `col-12`, `col-md-6`, `col-lg-4`.
