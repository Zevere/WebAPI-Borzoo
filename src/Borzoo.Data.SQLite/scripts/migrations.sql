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

CREATE UNIQUE INDEX IF NOT EXISTS idx_user_name ON user(name);
