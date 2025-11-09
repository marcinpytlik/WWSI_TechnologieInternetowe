import express from 'express'
import helmet from 'helmet'
import compression from 'compression'
import morgan from 'morgan'
import cookieParser from 'cookie-parser'
import fs from 'fs/promises'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const app = express()
const PORT = process.env.PORT || 5171

app.disable('x-powered-by')

app.use(helmet({
  contentSecurityPolicy: {
    useDefaults: true,
    directives: {
      "default-src": ["'self'"],
      "img-src": ["'self'", "data:"],
      "style-src": ["'self'", "'unsafe-inline'"],
      "script-src": ["'self'", "https://cdn.jsdelivr.net"],
      "connect-src": ["'self'", "https://cdn.jsdelivr.net"]
    }
  }
}))

app.use(compression())
app.use(express.json())
app.use(cookieParser())
app.use(morgan('dev'))
app.use(express.static(path.join(__dirname, 'public'), { maxAge: '1h', etag: true }))

const dataPath = path.join(__dirname, 'data', 'poll.json')

async function ensureData(){
  try { await fs.access(dataPath) } 
  catch { 
    await fs.mkdir(path.dirname(dataPath), { recursive: true })
    await fs.writeFile(dataPath, JSON.stringify({
      options: [
        { id: 1, label: 'React' }, { id: 2, label: 'Vue' }, { id: 3, label: 'Angular' }, { id: 4, label: 'Svelte' }, { id: 5, label: 'Solid' }
      ],
      votes: []
    }, null, 2)) 
  }
}
async function readData(){ await ensureData(); return JSON.parse(await fs.readFile(dataPath, 'utf-8')) }
async function writeData(obj){ await fs.writeFile(dataPath, JSON.stringify(obj, null, 2)) }

app.get('/api/options', async (req,res)=>{
  const db = await readData()
  res.set('Cache-Control','no-store')
  res.json(db.options)
})

app.post('/api/vote', async (req,res)=>{
  const { option_id } = req.body || {}
  const db = await readData()
  const opt = db.options.find(o=>o.id === option_id)
  if(!opt) return res.status(404).json({ error: 'option not found' })
  db.votes.push({ option_id, created_at: new Date().toISOString() })
  await writeData(db)
  res.status(201).json({ ok:true })
})

app.get('/api/results', async (req,res)=>{
  const db = await readData()
  const tally = db.options.map(o=>({ id:o.id, label:o.label, votes: db.votes.filter(v=>v.option_id===o.id).length }))
  tally.sort((a,b)=> b.votes - a.votes || a.id - b.id)
  res.set('Cache-Control','no-store')
  res.json(tally)
})

app.get('/api/health', (req,res)=> res.json({status:'ok'}))

app.use((req,res)=> res.status(404).json({ error:'Not Found' }))

app.listen(PORT, ()=> console.log('LivePoll listening on http://localhost:'+PORT))
