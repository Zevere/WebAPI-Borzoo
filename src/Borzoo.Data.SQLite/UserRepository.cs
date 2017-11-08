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
            string sql = $"INSERT INTO user(name, first_name, joined_at {(hasLastName ? ", last_name" : "")} ) " +
                         $"VALUES ($name, $fname, $time {(hasLastName ? ", $lname" : "")} ); " +
                         "SELECT last_insert_rowid() AS id";

            Connection.Open();
            using (var tnx = Connection.BeginTransaction())
            {
                var cmd = Connection.CreateCommand();
                cmd.Transaction = tnx;

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$name", entity.DisplayId);
                cmd.Parameters.AddWithValue("$fname", entity.FirstName);
                cmd.Parameters.AddWithValue("$time", entity.JoinedAt.ToUnixEpoch());
                if (hasLastName)
                    cmd.Parameters.AddWithValue("$lname", entity.LastName);

                entity.Id = cmd.ExecuteScalar().ToString();
                tnx.Commit();
            }
            return Task.FromResult(entity);
        }
    }
}