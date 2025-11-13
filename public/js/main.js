import { getHello, login, listTodos, createTodo } from './api.js'

const btn = document.getElementById('btn')
const list = document.getElementById('list')
const add = document.getElementById('add')
const out = document.getElementById('out')
const msg = document.getElementById('msg')
const form = document.getElementById('loginForm')

function show(o){ out.textContent = JSON.stringify(o, null, 2) }
function info(t){ msg.textContent = t }

form.addEventListener('submit', async (e) => {
  e.preventDefault()
  const data = Object.fromEntries(new FormData(form).entries())
  try{
    await login(data.username, data.password)
    info('Zalogowano.')
  }catch(err){
    info('Błąd logowania: ' + err.message)
  }
})

btn.addEventListener('click', async () => {
  info('Ładuję...')
  try{ show(await getHello()) } catch(e){ info(e.message) } finally { info('') }
})

list.addEventListener('click', async () => {
  info('Ładuję listę...')
  try{ show(await listTodos()) } catch(e){ info(e.message) } finally { info('') }
})

add.addEventListener('click', async () => {
  const title = prompt('Tytuł TODO:')
  if(!title) return
  info('Dodaję...')
  try{ show(await createTodo(title)) } catch(e){ info(e.message) } finally { info('') }
})
