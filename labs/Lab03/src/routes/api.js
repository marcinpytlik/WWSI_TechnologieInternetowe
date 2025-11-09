import { Router } from 'express';
import { db } from '../db.js';

const router = Router();

// -------- POSTS --------

// GET /api/posts
router.get('/posts', (req, res) => {
  const rows = db.prepare('SELECT id, title, body, created_at FROM posts ORDER BY id DESC').all();
  res.json(rows);
});

// POST /api/posts {title, body}
router.post('/posts', (req, res) => {
  const { title, body } = req.body || {};
  if (!title?.trim() || !body?.trim()) return res.status(400).json({ message: 'Invalid title or body.' });
  const now = new Date().toISOString();
  const info = db.prepare('INSERT INTO posts(title, body, created_at) VALUES(?,?,?)').run(title.trim(), body.trim(), now);
  res.status(201).location(`/api/posts/${info.lastInsertRowid}`).json({ id: info.lastInsertRowid, title: title.trim(), body: body.trim(), created_at: now });
});

// -------- COMMENTS --------

// GET /api/posts/:id/comments (approved only)
router.get('/posts/:id/comments', (req, res) => {
  const postId = parseInt(req.params.id);
  const post = db.prepare('SELECT id FROM posts WHERE id = ?').get(postId);
  if (!post) return res.status(404).json({ message: 'Post not found.' });
  const rows = db.prepare('SELECT id, author, body, created_at FROM comments WHERE post_id = ? AND approved = 1 ORDER BY id DESC').all(postId);
  res.json(rows);
});

// POST /api/posts/:id/comments {author, body} (approved=0)
router.post('/posts/:id/comments', (req, res) => {
  const postId = parseInt(req.params.id);
  const { author, body } = req.body || {};
  if (!author?.trim() || !body?.trim()) return res.status(400).json({ message: 'Invalid author or body.' });
  const post = db.prepare('SELECT id FROM posts WHERE id = ?').get(postId);
  if (!post) return res.status(404).json({ message: 'Post not found.' });
  const now = new Date().toISOString();
  const info = db.prepare('INSERT INTO comments(post_id, author, body, created_at, approved) VALUES(?,?,?,?,0)').run(postId, author.trim(), body.trim(), now);
  res.status(201).location(`/api/comments/${info.lastInsertRowid}`).json({ id: info.lastInsertRowid, approved: 0 });
});

// POST /api/comments/:id/approve
router.post('/comments/:id/approve', (req, res) => {
  const id = parseInt(req.params.id);
  const row = db.prepare('SELECT id, approved FROM comments WHERE id = ?').get(id);
  if (!row) return res.status(404).json({ message: 'Comment not found.' });
  if (row.approved === 1) return res.json({ message: 'Already approved.' });
  db.prepare('UPDATE comments SET approved = 1 WHERE id = ?').run(id);
  res.json({ message: 'Approved.' });
});

export default router;
