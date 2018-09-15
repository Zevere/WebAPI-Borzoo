-- DROP INDEX IF EXISTS idx_user_name;
-- DROP TABLE IF EXISTS user;

CREATE TABLE IF NOT EXISTS user (
  id          INTEGER PRIMARY KEY,
  name        TEXT    NOT NULL,
  passphrase  TEXT    NOT NULL,
  first_name  TEXT    NOT NULL,
  last_name   TEXT,
  joined_at   INTEGER NOT NULL DEFAULT (CAST(strftime('%s', 'now') AS INTEGER)), -- Unix epoch time
  modified_at INTEGER, -- Unix epoch time
  is_deleted  INTEGER -- Non-NULL values indicate Deleted
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_user_name
  ON user (name);

CREATE TABLE IF NOT EXISTS login (
  user_id     INTEGER NOT NULL,
  token       TEXT    NOT NULL,
  created_at  INTEGER NOT NULL DEFAULT (CAST(strftime('%s', 'now') AS INTEGER)), -- Unix epoch time
  modified_at INTEGER, -- Unix epoch time
  FOREIGN KEY (user_id) REFERENCES user (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_login_user_id
  ON login (user_id);

CREATE TABLE IF NOT EXISTS tasklist (
  id            INTEGER PRIMARY KEY,
  owner_id      INTEGER NOT NULL,
  name          TEXT    NOT NULL,
  title         TEXT    NOT NULL,
  created_at    INTEGER NOT NULL DEFAULT (CAST(strftime('%s', 'now') AS INTEGER)), -- Unix epoch time
  is_deleted  INTEGER, -- Non-NULL values indicate Deleted
  FOREIGN KEY (owner_id) REFERENCES user (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_tasklist_ownerId_name
  ON tasklist (owner_id, name);

CREATE TABLE IF NOT EXISTS task (
  id            INTEGER PRIMARY KEY,
  name          TEXT    NOT NULL,
  list_id       INTEGER NOT NULL,
  title         TEXT    NOT NULL,
  description   TEXT,
  due           INTEGER, -- Unix epoch time
  created_at    INTEGER NOT NULL DEFAULT (CAST(strftime('%s', 'now') AS INTEGER)), -- Unix epoch time
  modified_at   INTEGER, -- Unix epoch time
  is_deleted    INTEGER, -- Non-NULL values indicate Deleted
  FOREIGN KEY (list_id) REFERENCES tasklist (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_task_listId_name
  ON task (list_id, name);
