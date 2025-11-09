const BASE = 'http://localhost:4000'

function withTimeout(ms, p){
  const ctrl = new AbortController()
  const t = setTimeout(() => ctrl.abort(), ms)
  return Promise.race([
    p(ctrl.signal).finally(() => clearTimeout(t)),
    new Promise((_,rej) => setTimeout(() => rej(new Error('timeout')), ms+10))
  ])
}

async function http(path, opts = {}, signal){
  const res = await fetch(BASE + path, { credentials: 'include', ...opts, signal })
  if(!res.ok){
    const text = await res.text()
    throw new Error(`HTTP ${res.status} ${text}`)
  }
  const ct = res.headers.get('content-type') || ''
  if(ct.includes('application/json')) return res.json()
  return res.text()
}

export async function getHello(){ 
  return withTimeout(5000, (signal)=> http('/api/hello', {}, signal))
}

export async function login(username, password){
  return withTimeout(5000, (signal)=> http('/api/login', {
    method:'POST',
    headers:{'Content-Type':'application/json'},
    body: JSON.stringify({username,password})
  }, signal))
}

export async function listTodos(){
  return withTimeout(5000, (signal)=> http('/api/todos?page=1&size=10', {}, signal))
}

export async function createTodo(title){
  return withTimeout(5000, (signal)=> http('/api/todos', {
    method:'POST',
    headers:{'Content-Type':'application/json'},
    body: JSON.stringify({title})
  }, signal))
}
