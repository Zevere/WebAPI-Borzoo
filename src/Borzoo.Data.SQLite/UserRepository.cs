using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.SQLite
{
    public class UserRepository : EntityRepositoryBase, IEntityRepository<User>
    {
        public UserRepository(string connectionString, string creationScriptFile)
            : base(connectionString, creationScriptFile)
        {
        }

        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken) // = default
        {
            bool hasLastName = !string.IsNullOrWhiteSpace(entity.LastName);
            string sql = $"INSERT INTO user(name, first_name, joined_at {(hasLastName ? ", last_name" : "")} ) " +
                         $"VALUES ($name, $fname, $time {(hasLastName ? ", $lname" : "")} )";

            using (var conn = CreateConnection())
            {
                var cmd = conn.CreateCommand();

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$name", entity.DisplayId);
                cmd.Parameters.AddWithValue("$fname", entity.FirstName);
                cmd.Parameters.AddWithValue("$time", entity.JoinedAt.ToUnixEpoch());
                if (hasLastName)
                    cmd.Parameters.AddWithValue("$lname", entity.LastName);

                await conn.OpenAsync(cancellationToken);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                entity.Id = await conn.GetLastInsertedRowIdAsync(cancellationToken);
            }
            return entity;
        }
    }
}