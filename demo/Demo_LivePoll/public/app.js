const $ = s => document.querySelector(s)
const statusEl = $('#status')

async function api(path, opts = {}) {
  const res = await fetch(path, {
    ...opts,
    headers: { 'Content-Type': 'application/json', ...(opts.headers || {}) }
  })
  if (!res.ok) throw new Error('HTTP ' + res.status)
  return res.json()
}

async function init() {
  try {
    await api('/api/health')
    statusEl.textContent = 'API: online'
  } catch {
    statusEl.textContent = 'API: offline'
  }
  renderOptions()
  startChart()
  setInterval(updateChart, 2000)
}

async function renderOptions() {
  const opts = await api('/api/options')
  const ul = $('#options')
  ul.innerHTML = ''
  opts.forEach(o => {
    const li = document.createElement('li')
    const btn = document.createElement('button')
    btn.textContent = 'Głosuj'
    btn.onclick = async () => {
      btn.disabled = true
      try {
        await api('/api/vote', { method: 'POST', body: JSON.stringify({ option_id: o.id }) })
      } finally {
        setTimeout(() => btn.disabled = false, 600)
      }
    }
    li.innerHTML = `<strong>${o.label}</strong>`
    li.appendChild(btn)
    ul.appendChild(li)
  })
}

let chart, chartData = { labels: [], values: [] }

async function startChart() {
  const ctx = document.getElementById('chart').getContext('2d')
  await updateChart()
  chart = new Chart(ctx, {
    type: 'bar',
    data: { labels: chartData.labels, datasets: [{ label: 'Głosy', data: chartData.values }] },
    options: { responsive: true, animation: false, scales: { y: { beginAtZero: true, precision: 0 } } }
  })
}

async function updateChart() {
  const data = await api('/api/results')
  chartData.labels = data.map(x => x.label)
  chartData.values = data.map(x => x.votes)
  if (chart) {
    chart.data.labels = chartData.labels
    chart.data.datasets[0].data = chartData.values
    chart.update()
  }
}

init()
