using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public class UserRepository : EntityRepositoryBase, IEntityRepository<User>
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

        public Task<User> GetAsync(string id, bool includeDeletedRecords = false,
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
                    throw new Exception("Not found!");

                entity = new User
                {
                    Id = id,
                    DisplayId = reader["name"].ToString(),
                    PassphraseHash = reader["passphrase"].ToString(),
                    FirstName = reader["first_name"].ToString(),
                    LastName = reader["last_name"] as string,
                    JoinedAt = long.Parse(reader["joined_at"].ToString()).FromUnixEpoch()
                };

                string modifiedAtValue = reader["modified_at"] as string;
                if (modifiedAtValue != null)
                {
                    entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixEpoch();
                }
            }

            return Task.FromResult(entity);
        }
    }
}