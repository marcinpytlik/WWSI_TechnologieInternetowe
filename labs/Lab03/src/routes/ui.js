import { Router } from 'express';
import { db } from '../db.js';

const router = Router();

// Home: list posts
router.get('/', (req, res) => {
  const posts = db.prepare('SELECT id, title, created_at FROM posts ORDER BY id DESC').all();
  res.type('html').send(`<!DOCTYPE html>
<html lang="pl"><head><meta charset="utf-8"/><title>Blog</title>
<link rel="stylesheet" href="/public/site.css"/></head>
<body>
<header><h1>Lab03 — Blog</h1>
<nav><a href="/">Posty</a> | <a href="/new">Nowy post</a> | <a href="/moderation">Moderacja</a></nav>
</header>
<section>
<table><thead><tr><th>Tytuł</th><th>Data</th><th>Akcja</th></tr></thead>
<tbody>
${posts.map(p=>`<tr>
  <td>${escapeHtml(p.title)}</td>
  <td>${new Date(p.created_at).toLocaleString()}</td>
  <td><a href="/posts/${p.id}">Komentarze</a></td>
</tr>`).join('')}
${posts.length===0?'<tr><td colspan="3"><i>Brak postów</i></td></tr>':''}
</tbody></table>
</section>
</body></html>`);
});

// New post form
router.get('/new', (req, res) => {
  res.type('html').send(`<!DOCTYPE html>
<html lang="pl"><head><meta charset="utf-8"/><title>Nowy post</title>
<link rel="stylesheet" href="/public/site.css"/></head>
<body>
<header><h1>Nowy post</h1><nav><a href="/">Posty</a> | <a href="/moderation">Moderacja</a></nav></header>
<section>
<form method="post" action="/posts/create">
  <p><label>Tytuł: <input name="title" required style="width:400px"/></label></p>
  <p><label>Treść:<br/><textarea name="body" rows="8" cols="80" required></textarea></label></p>
  <button>Dodaj</button>
</form>
</section>
</body></html>`);
});

router.post('/posts/create', (req, res) => {
  const { title, body } = req.body || {};
  if (title?.trim() && body?.trim()) {
    const now = new Date().toISOString();
    db.prepare('INSERT INTO posts(title,body,created_at) VALUES(?,?,?)').run(title.trim(), body.trim(), now);
  }
  res.redirect('/');
});

// Post details with approved comments + add form
router.get('/posts/:id', (req, res) => {
  const id = parseInt(req.params.id);
  const post = db.prepare('SELECT * FROM posts WHERE id = ?').get(id);
  if (!post) return res.status(404).send('Not found');
  const comments = db.prepare('SELECT id, author, body, created_at FROM comments WHERE post_id = ? AND approved = 1 ORDER BY id DESC').all(id);
  const pendingCount = db.prepare('SELECT COUNT(*) AS c FROM comments WHERE post_id = ? AND approved = 0').get(id).c;

  res.type('html').send(`<!DOCTYPE html>
<html lang="pl"><head><meta charset="utf-8"/><title>${escapeHtml(post.title)}</title>
<link rel="stylesheet" href="/public/site.css"/></head>
<body>
<header><h1>${escapeHtml(post.title)}</h1>
<nav><a href="/">Posty</a> | <a href="/moderation">Moderacja (${pendingCount})</a></nav>
</header>
<article><pre>${escapeHtml(post.body)}</pre></article>
<section>
  <h2>Komentarze (zatwierdzone)</h2>
  <ul>
    ${comments.map(c=>`<li><b>${escapeHtml(c.author)}</b> (${new Date(c.created_at).toLocaleString()}):<br/>${escapeHtml(c.body)}</li>`).join('') || '<i>Brak komentarzy</i>'}
  </ul>
</section>
<section>
  <h3>Dodaj komentarz (trafi do moderacji)</h3>
  <form method="post" action="/posts/${post.id}/comments/create">
    <p><label>Autor: <input name="author" required/></label></p>
    <p><label>Treść:<br/><textarea name="body" rows="5" cols="80" required></textarea></label></p>
    <button>Wyślij</button>
  </form>
</section>
</body></html>`);
});

router.post('/posts/:id/comments/create', (req, res) => {
  const id = parseInt(req.params.id);
  const { author, body } = req.body || {};
  if (author?.trim() && body?.trim()) {
    const now = new Date().toISOString();
    db.prepare('INSERT INTO comments(post_id,author,body,created_at,approved) VALUES(?,?,?,?,0)').run(id, author.trim(), body.trim(), now);
  }
  res.redirect(`/posts/${id}`);
});

// Moderation panel
router.get('/moderation', (req, res) => {
  const rows = db.prepare(`
    SELECT c.id, c.post_id, p.title AS post_title, c.author, c.body, c.created_at
    FROM comments c JOIN posts p ON p.id = c.post_id
    WHERE c.approved = 0 ORDER BY c.id ASC
  `).all();

  res.type('html').send(`<!DOCTYPE html>
<html lang="pl"><head><meta charset="utf-8"/><title>Moderacja</title>
<link rel="stylesheet" href="/public/site.css"/></head>
<body>
<header><h1>Moderacja komentarzy</h1><nav><a href="/">Posty</a></nav></header>
<section>
  <table><thead><tr><th>Post</th><th>Autor</th><th>Komentarz</th><th>Data</th><th>Akcja</th></tr></thead>
  <tbody>
    ${rows.map(r=>`<tr>
      <td><a href="/posts/${r.post_id}">${escapeHtml(r.post_title)}</a></td>
      <td>${escapeHtml(r.author)}</td>
      <td>${escapeHtml(r.body)}</td>
      <td>${new Date(r.created_at).toLocaleString()}</td>
      <td>
        <form method="post" action="/comments/${r.id}/approve">
          <button>Zatwierdź</button>
        </form>
      </td>
    </tr>`).join('') || '<tr><td colspan="5"><i>Brak oczekujących</i></td></tr>'}
  </tbody></table>
</section>
</body></html>`);
});

router.post('/comments/:id/approve', (req, res) => {
  const id = parseInt(req.params.id);
  db.prepare('UPDATE comments SET approved = 1 WHERE id = ?').run(id);
  res.redirect('/moderation');
});

function escapeHtml(s) {
  return String(s || '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

export default router;
