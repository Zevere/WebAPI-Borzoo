using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.SQLite
{
    public class UserRepository : EntityRepositoryBase, IEntityRepository<User>
    {
        public Task<User> CreateAsync(User entity, CancellationToken cancellationToken)
        {
            bool hasLastName = !string.IsNullOrWhiteSpace(entity.LastName);
            string sql = $"INSERT INTO user(name, first_name, joined_at {(hasLastName ? ", last_name" : "")} ) " +
                         $"VALUES ($name, $fname, $time {(hasLastName ? ", $lname" : "")} )";

            using (var conn = CreateConnection())
            {
                conn.Open();
                using (var tnx = conn.BeginTransaction())
                {
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = tnx;

                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("$name", entity.DisplayId);
                    cmd.Parameters.AddWithValue("$fname", entity.FirstName);
                    cmd.Parameters.AddWithValue("$time", entity.JoinedAt.ToUnixEpoch());
                    if (hasLastName)
                        cmd.Parameters.AddWithValue("$lname", entity.LastName);

                    cmd.ExecuteNonQuery();
                    entity.Id = conn.GetLastInsertedRowId(tnx);
                    tnx.Commit();
                }
            }
            return Task.FromResult(entity);
        }
    }
}