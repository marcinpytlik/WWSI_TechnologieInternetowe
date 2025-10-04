# ⚡ JavaScript (ES6+) – Kompendium

## 1) Podstawy składni
```js
// Zmienne
let a = 1;          // re-assign OK, block scope
const PI = 3.14;    // bez re-assign
var x = 0;          // (nie używaj) function scope

// Typy prymitywne
const n = 42;                  // number (także NaN, Infinity)
const b = true;                // boolean
const s = "txt";               // string
const big = 123n;              // bigint
const v = undefined;
const z = null;
const sym = Symbol("id");

// Template literals
const who = "Marcin";
console.log(`Hej ${who}!`);
```

## 2) Operatory i kontrola przepływu
```js
if (a > 10) { ... } else if (...) { ... } else { ... }
for (let i = 0; i < 3; i++) { ... }
for (const v of [1,2,3]) { ... }  // po wartościach
for (const k in obj) { ... }      // po kluczach obiektu
while (cond) { ... }
switch (key) { case "x": ...; break; default: ... }

// Operatory
a === b;  // porównanie ścisłe
a ?? b;   // nullish coalescing (gdy a == null/undefined → b)
a || b;   // OR (z krótkim spięciem)
a && b;   // AND
a?.b?.c;  // optional chaining
```

## 3) Funkcje, zakres i `this`
```js
function sum(a, b = 0) { return a + b; }
const mul = (a, b) => a * b;         // arrow – brak własnego `this`, `arguments`

// `this` zależy od sposobu wywołania
const obj = {
  x: 10,
  reg() { return this.x; },
  arr: () => this // `this` z otaczającego leksykalnego kontekstu
};
```

## 4) Obiekty, destrukturyzacja, spread/rest
```js
const user = { id: 1, name: "Ala", role: "admin" };
const { name, role: r, age = 0 } = user; // destrukturyzacja + alias + default
const more = { ...user, active: true };  // spread

const nums = [1,2,3];
const [first, ...rest] = nums;           // rest w tablicach
```

## 5) Tablice – metody praktyczne
```js
const arr = [1,2,3,4];
arr.map(x => x * 2);            // [2,4,6,8]
arr.filter(x => x % 2 === 0);   // [2,4]
arr.reduce((acc, x) => acc + x, 0); // 10
arr.find(x => x > 2);           // 3
arr.some(x => x > 3);           // true
arr.every(x => x > 0);          // true
arr.includes(3);                // true
```

## 6) Klasy i prototypy
```js
class Person {
  constructor(name) { this.name = name; }
  hello() { return `Cześć, ${this.name}`; }
  static of(name) { return new Person(name); }
}
class Dev extends Person {
  hello() { return super.hello() + " 👨‍💻"; }
}
```

## 7) Moduły (ES Modules)
```js
// math.js
export const add = (a,b) => a+b;
export default function mul(a,b){ return a*b; }

// main.js
import mul, { add } from "./math.js";
```

## 8) Promise, async/await, fetch
```js
// Promise
const p = new Promise((resolve, reject) => {
  setTimeout(() => resolve(42), 100);
});
p.then(v => console.log(v)).catch(console.error);

// async/await
async function work() {
  try {
    const v = await p;
    console.log(v);
  } catch (e) { /* obsługa */ }
}

// fetch (przeglądarka / node >=18)
async function getUsers() {
  const res = await fetch("https://jsonplaceholder.typicode.com/users");
  if (!res.ok) throw new Error(res.statusText);
  return await res.json();
}
```

## 9) DOM i zdarzenia (przeglądarka)
```html
<button id="btn">Klik</button>
<div id="out"></div>
<script type="module">
  const btn = document.getElementById("btn");
  const out = document.querySelector("#out");
  btn.addEventListener("click", () => { out.textContent = "Bang!"; });
</script>
```

## 10) Storage i JSON
```js
localStorage.setItem("token", "abc");
const t = localStorage.getItem("token");
localStorage.removeItem("token");

const data = { id: 1, name: "Ala" };
const sjson = JSON.stringify(data);      // obiekt → string
const back = JSON.parse(sjson);          // string → obiekt
```

## 11) Błędy i debug
```js
try {
  risky();
} catch (e) {
  console.error(e.name, e.message);
} finally {
  // sprzątanie
}
```

## 12) Struktury danych wbudowane
```js
const set = new Set([1,2,2,3]);     // {1,2,3}
const map = new Map([["a",1],["b",2]]);
set.has(2); set.add(4); set.delete(1);
map.get("a"); map.set("c",3); map.delete("b");
```

## 13) Daty, Intl, RegExp
```js
const d = new Date();
d.toISOString();

const fmt = new Intl.NumberFormat("pl-PL", { style: "currency", currency: "PLN" });
fmt.format(1234.56);

const re = /\b[A-Za-z]{3,}\b/g;
"ala ma kota".match(re);
```

## 14) Node.js – szybki start
```bash
npm init -y
npm install express
```
```js
// index.js
import express from "express";
const app = express();
app.get("/", (_,res)=>res.send("Działa"));
app.listen(3000);
```

## 15) Patterny na co dzień
```js
// Guard Clauses
function createUser(u){
  if(!u?.email) throw new Error("email required");
  return { ...u, createdAt: new Date() };
}

// Immutability helpers
const state = { users: [] };
const newState = { ...state, users: [...state.users, { id: 1 }] };
```

## 16) Najczęstsze pułapki
- Używaj `===` zamiast `==`.  
- Pamiętaj, że `NaN !== NaN` (sprawdź `Number.isNaN`).  
- `this` w arrow nie wiąże się do obiektu – użyj zwykłej metody, jeśli potrzebujesz `this`.  
- Nie mutuj tablic/obiektów w React/Redux – twórz kopie (`...spread`).  

---

### Mini-lab: fetch + render
```html
<ul id="users"></ul>
<script type="module">
  const ul = document.getElementById("users");
  const res = await fetch("https://jsonplaceholder.typicode.com/users");
  const users = await res.json();
  ul.innerHTML = users.map(u => `<li>${u.name} – ${u.email}</li>`).join("");
</script>
```
