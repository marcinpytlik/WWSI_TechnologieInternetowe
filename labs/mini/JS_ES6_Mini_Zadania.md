# ⚡ JavaScript (ES6+) – Mini zadania

## 1. Zmienne i typy
1. Utwórz zmienną `const imie = "Marcin";` i wypisz w konsoli komunikat:  
   ```
   Witaj, Marcin!
   ```
   używając **template literal**.  
2. Zadeklaruj zmienną `let liczba = 42;` i sprawdź jej typ przy pomocy `typeof`.

---

## 2. Operatory
1. Użyj operatora `??` tak, aby zmienna `wynik` przyjmowała wartość `"domyślna"` jeśli `wartosc` jest `null`.  
2. Zbuduj warunek, który wypisze „pełnoletni” gdy `wiek >= 18`.

---

## 3. Funkcje
1. Napisz funkcję `square`, która przyjmuje liczbę i zwraca jej kwadrat.  
2. Zamień ją na funkcję strzałkową (`arrow function`).  

---

## 4. Destrukturyzacja
Masz obiekt:  
```js
const user = { id: 7, name: "Ala", role: "admin" };
```
1. Wyciągnij `name` i `role` do osobnych zmiennych przy pomocy **destrukturyzacji**.  
2. Zrób kopię obiektu z dodatkowym polem `active: true` używając **spread operatora**.

---

## 5. Tablice
Masz tablicę:  
```js
const nums = [2, 5, 7, 10];
```
1. Utwórz nową tablicę z wartościami podwojonymi (`map`).  
2. Wybierz tylko liczby parzyste (`filter`).  
3. Policz sumę wszystkich elementów (`reduce`).  

---

## 6. Klasy
1. Zdefiniuj klasę `Osoba` z polem `imie` i metodą `powitanie()`, która zwraca `Cześć, imie`.  
2. Rozszerz klasę o `Pracownik`, który w powitaniu dodaje `– jestem pracownikiem`.

---

## 7. Moduły
1. Utwórz plik `math.js` z funkcją `dodaj(a,b)`.  
2. Zaimportuj ją w `index.js` i użyj do obliczenia sumy `2 + 3`.

---

## 8. Fetch + async/await
1. Pobierz dane z API: `https://jsonplaceholder.typicode.com/todos/1`.  
2. Wyświetl w konsoli tytuł zadania (`title`).  

---

## 9. DOM i zdarzenia
1. Utwórz w HTML przycisk `<button id="klik">Kliknij</button>`.  
2. W JS dodaj event listener, który po kliknięciu wyświetli `alert("Kliknięto!")`.  

---

## 10. LocalStorage
1. Zapisz do `localStorage` klucz `"user"` z wartością `"Marcin"`.  
2. Po odświeżeniu strony odczytaj tę wartość i wypisz w konsoli.  
