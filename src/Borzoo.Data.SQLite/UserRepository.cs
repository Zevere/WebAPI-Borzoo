using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Borzoo.Data.SQLite
{
    public class UserRepository : EntityRepositoryBase, IUserRepository
    {
        public UserRepository()
        {
        }

        public UserRepository(string connectionString)
            : base(connectionString)
        {
        }

        public UserRepository(SqliteConnection connection)
            : base(connection)
        {
        }

        public Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            bool hasLastName = !string.IsNullOrWhiteSpace(entity.LastName);
            string sql =
                $"INSERT INTO user(name, passphrase, first_name, joined_at {(hasLastName ? ", last_name" : "")} ) " +
                $"VALUES ($name, $passphrase, $fname, $time {(hasLastName ? ", $lname" : "")} ); " +
                "SELECT last_insert_rowid() AS id";

            using (var tnx = Connection.BeginTransaction())
            {
                var cmd = Connection.CreateCommand();
                cmd.Transaction = tnx;

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$name", entity.DisplayId);
                cmd.Parameters.AddWithValue("$passphrase", entity.PassphraseHash);
                cmd.Parameters.AddWithValue("$fname", entity.FirstName);
                cmd.Parameters.AddWithValue("$time", entity.JoinedAt.ToUnixTime());
                if (hasLastName)
                    cmd.Parameters.AddWithValue("$lname", entity.LastName);

                try
                {
                    entity.Id = cmd.ExecuteScalar().ToString();
                }
                catch (SqliteException e) when
                (e.SqliteErrorCode == raw.SQLITE_CONSTRAINT &&
                 e.Message.Contains("user.name"))
                {
                    throw new DuplicateKeyException(nameof(User.DisplayId));
                }

                if (cancellationToken.IsCancellationRequested)
                    throw new TaskCanceledException();
                tnx.Commit();
            }

            return Task.FromResult(entity);
        }

        public Task<User> GetByIdAsync(string id, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            string sql = "SELECT name, passphrase, first_name, last_name, joined_at, modified_at, is_deleted " +
                         "FROM user " +
                         "WHERE id = $id ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            var cmd = Connection.CreateCommand();

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$id", id);

            User entity;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!(reader.HasRows && reader.Read()))
                {
                    throw new EntityNotFoundException(id);
                }

                entity = new User
                {
                    Id = id,
                    DisplayId = reader["name"].ToString(),
                    PassphraseHash = reader["passphrase"].ToString(),
                    FirstName = reader["first_name"].ToString(),
                    LastName = reader["last_name"] as string,
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixTime(),
                    IsDeleted = !string.IsNullOrWhiteSpace(reader["is_deleted"].ToString())
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixTime();
                }
            }

            return Task.FromResult(entity);
        }

        public Task<User> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            string sql = "SELECT id, passphrase, first_name, last_name, joined_at, modified_at, is_deleted " +
                         "FROM user " +
                         "WHERE UPPER(name) = UPPER($name) ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            var cmd = Connection.CreateCommand();

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$name", name);

            User entity;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!(reader.HasRows && reader.Read()))
                {
                    throw new EntityNotFoundException("User Name", name);
                }

                entity = new User
                {
                    DisplayId = name.ToLower(),
                    Id = reader["id"].ToString(),
                    PassphraseHash = reader["passphrase"].ToString(),
                    FirstName = reader["first_name"].ToString(),
                    LastName = reader["last_name"] as string,
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixTime(),
                    IsDeleted = !string.IsNullOrWhiteSpace(reader["is_deleted"].ToString())
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixTime();
                }
            }

            return Task.FromResult(entity);
        }

        public Task<User> GetByTokenAsync(string token, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            string sql = "SELECT id, name, passphrase, first_name, last_name, joined_at, u.modified_at, is_deleted " +
                         "FROM user u LEFT OUTER JOIN login l ON (u.id = l.user_id) " +
                         "WHERE token = $token ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            var cmd = Connection.CreateCommand();

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$token", token);

            User entity;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!(reader.HasRows && reader.Read()))
                {
                    throw new EntityNotFoundException("Token", token);
                }

                entity = new User
                {
                    Id = reader["id"].ToString(),
                    DisplayId = reader["name"].ToString().ToLower(),
                    PassphraseHash = reader["passphrase"].ToString(),
                    FirstName = reader["first_name"].ToString(),
                    LastName = reader["last_name"] as string,
                    Token = token,
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixTime(),
                    IsDeleted = !string.IsNullOrWhiteSpace(reader["is_deleted"].ToString())
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixTime();
                }
            }

            return Task.FromResult(entity);
        }

        public Task<User> GetByPassphraseLoginAsync(string userName, string passphrase,
            bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            string sql = "SELECT id, first_name, last_name, joined_at, u.modified_at, is_deleted, token " +
                         "FROM user u LEFT OUTER JOIN login l ON (u.id = l.user_id) " +
                         "WHERE UPPER(name) = UPPER($name) AND passphrase = $passphrase ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            var cmd = Connection.CreateCommand();

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$name", userName);
            cmd.Parameters.AddWithValue("$passphrase", passphrase);

            User entity;
            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (!(reader.HasRows && reader.Read()))
                {
                    throw new EntityNotFoundException("Name", userName);
                }

                entity = new User
                {
                    Id = reader["id"].ToString(),
                    DisplayId = userName,
                    PassphraseHash = passphrase,
                    FirstName = reader["first_name"].ToString(),
                    LastName = reader["last_name"] as string,
                    Token = reader["token"] as string,
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixTime(),
                    IsDeleted = !string.IsNullOrWhiteSpace(reader["is_deleted"].ToString())
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixTime();
                }
            }

            return Task.FromResult(entity);
        }

        public Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            bool includesLastName = !string.IsNullOrWhiteSpace(entity.LastName);

            string sql =
                "UPDATE user SET " +
                "name = $name, passphrase = $passphrase, first_name = $fname, " +
                (includesLastName ? "last_name = $lname, " : string.Empty) +
                "modified_at = $modified_at " +
                "WHERE id = $id";

            var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;

            var modifiedTime = entity.ModifiedAt ?? DateTime.UtcNow;

            cmd.Parameters.AddWithValue("$id", entity.Id);
            cmd.Parameters.AddWithValue("$name", entity.DisplayId);
            cmd.Parameters.AddWithValue("$passphrase", entity.PassphraseHash);
            cmd.Parameters.AddWithValue("$fname", entity.FirstName);
            cmd.Parameters.AddWithValue("$modified_at", modifiedTime.ToUnixTime());
            if (includesLastName)
            {
                cmd.Parameters.AddWithValue("$lname", entity.LastName);
            }

            cmd.ExecuteNonQuery();

            entity.ModifiedAt = modifiedTime.ToUniversalTime();

            return Task.FromResult(entity);
        }

        public Task SetTokenForUserAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(user_id) FROM login WHERE user_id = $user_id";
            cmd.Parameters.AddWithValue("$user_id", userId);
            bool loginExists = int.Parse(cmd.ExecuteScalar().ToString()) == 1;

            if (loginExists)
            {
                cmd.CommandText = "UPDATE login " +
                                  "SET token = $token, modified_at = $modified_at " +
                                  "WHERE user_id = $user_id";
                cmd.Parameters.AddWithValue("$modified_at", DateTime.UtcNow.ToUnixTime());
            }
            else
            {
                cmd.CommandText = "INSERT INTO login(user_id, token) " +
                                  "VALUES($user_id, $token)";
            }

            cmd.Parameters.AddWithValue("$token", token);

            cmd.ExecuteNonQuery();

            return Task.CompletedTask;
        }

        public Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = "DELETE FROM login WHERE token = $token";
            cmd.Parameters.AddWithValue("$token", token);

            int rowsAffected = cmd.ExecuteNonQuery();

            return Task.FromResult(rowsAffected == 1);
        }

        public Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = (hardDelete ? "DELETE FROM user" : "UPDATE user SET is_deleted = 1") +
                                  " WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", id.ToLower());
                int count = cmd.ExecuteNonQuery();
                if (count == 0)
                {
                    throw new EntityNotFoundException(id);
                }

                cmd.CommandText = "DELETE FROM login WHERE user_id = $id";
                cmd.ExecuteNonQuery();
            }

            return Task.CompletedTask;
        }
    }
}