using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;

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

        public Task<User> AddAsync(User entity, CancellationToken cancellationToken)
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
                cmd.Parameters.AddWithValue("$time", entity.JoinedAt.ToUnixEpoch());
                if (hasLastName)
                    cmd.Parameters.AddWithValue("$lname", entity.LastName);

                entity.Id = cmd.ExecuteScalar().ToString();
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
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixEpoch()
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixEpoch();
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
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixEpoch()
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixEpoch();
                }
            }

            return Task.FromResult(entity);
        }

        public Task<User> GetByTokenAsync(string token, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            string sql = "SELECT id, name, passphrase, first_name, last_name, joined_at, u.modified_at, is_deleted " +
                         "FROM user u JOIN user_login l ON (u.id = l.user_id) " +
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
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixEpoch(),
                    IsDeleted = reader["is_deleted"] is string
                };

                string modifiedAtValue = reader["modified_at"].ToString();
                if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixEpoch();
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
            cmd.Parameters.AddWithValue("$modified_at", modifiedTime.ToUnixEpoch());
            if (includesLastName)
            {
                cmd.Parameters.AddWithValue("$lname", entity.LastName);
            }

            cmd.ExecuteNonQuery();

            entity.ModifiedAt = modifiedTime;

            return Task.FromResult(entity);
        }

        public Task SetTokenForUserAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(user_id) FROM user_login WHERE user_id = $user_id";
            cmd.Parameters.AddWithValue("$user_id", userId);
            bool loginExists = int.Parse(cmd.ExecuteScalar().ToString()) == 1;

            if (loginExists)
            {
                cmd.CommandText = "UPDATE user_login " +
                                  "SET token = $token, modified_at = $modified_at " +
                                  "WHERE user_id = $user_id";
                cmd.Parameters.AddWithValue("$modified_at", DateTime.UtcNow.ToUnixEpoch());
            }
            else
            {
                cmd.CommandText = "INSERT INTO user_login(user_id, token) " +
                                  "VALUES($user_id, $token)";
            }
            cmd.Parameters.AddWithValue("$token", token);

            cmd.ExecuteNonQuery();

            return Task.CompletedTask;
        }
    }
}