import Database from 'better-sqlite3'
import path from 'path'
import { fileURLToPath } from 'url'
import fs from 'fs'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const dbPath = path.join(__dirname, '..', 'poll.sqlite')

if (fs.existsSync(dbPath)) fs.rmSync(dbPath)
const db = new Database(dbPath)
db.exec(`
  PRAGMA foreign_keys = ON;
  CREATE TABLE options (id INTEGER PRIMARY KEY AUTOINCREMENT, label TEXT NOT NULL UNIQUE);
  CREATE TABLE votes (id INTEGER PRIMARY KEY AUTOINCREMENT, option_id INTEGER NOT NULL REFERENCES options(id) ON DELETE CASCADE, created_at TEXT NOT NULL);
  INSERT INTO options(label) VALUES ('React'),('Vue'),('Angular'),('Svelte'),('Solid');
`)
db.close()
console.log('DB reset ok')
