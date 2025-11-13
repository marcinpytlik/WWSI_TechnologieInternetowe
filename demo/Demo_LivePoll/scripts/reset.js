import fs from 'fs/promises'
import path from 'path'
import { fileURLToPath } from 'url'
const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const dataPath = path.join(__dirname, '..', 'data', 'poll.json')
await fs.mkdir(path.dirname(dataPath), { recursive: true })
await fs.writeFile(dataPath, JSON.stringify({
  options: [
    { id: 1, label: 'React' }, { id: 2, label: 'Vue' }, { id: 3, label: 'Angular' }, { id: 4, label: 'Svelte' }, { id: 5, label: 'Solid' }
  ],
  votes: []
}, null, 2))
console.log('Data reset: data/poll.json')
