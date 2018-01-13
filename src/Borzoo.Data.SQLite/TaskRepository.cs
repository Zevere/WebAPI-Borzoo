using System;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public class TaskRepository : EntityRepositoryBase, ITaskRepository
    {
        public string UserName
        {
            get => _userName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException();

                UserId = GetUserId(value);
                _userName = value;
            }
        }

        public string UserId { get; private set; }

        private string _userName;

        public TaskRepository()
        {
        }

        public TaskRepository(string connectionString)
            : base(connectionString)
        {
        }

        public TaskRepository(SqliteConnection connection)
            : base(connection)
        {
        }

        public Task<UserTask> AddAsync(UserTask task, CancellationToken cancellationToken = default)
        {
            EnsureUserId();
            bool hasDescription = !string.IsNullOrWhiteSpace(task.Description);
            bool hasDueDate = task.Due != default;

            string sql = "INSERT INTO task(user_id, title, created_at" +
                         (hasDescription ? ", description" : string.Empty) +
                         (hasDueDate ? ", due" : string.Empty) +
                         ")" +
                         "VALUES ($user_id, $title, $created_at" +
                         (hasDescription ? ", $description" : string.Empty) +
                         (hasDueDate ? ", $due" : string.Empty) +
                         ");" +
                         "SELECT last_insert_rowid() AS id";

            using (var tnx = Connection.BeginTransaction())
            {
                using (var cmd = Connection.CreateCommand())
                {
                    cmd.Transaction = tnx;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("$user_id", UserId);
                    cmd.Parameters.AddWithValue("$title", task.Title);
                    cmd.Parameters.AddWithValue("$created_at", task.CreatedAt.ToUnixTime());
                    if (hasDescription)
                        cmd.Parameters.AddWithValue("$description", task.Description);
                    if (hasDueDate)
                        cmd.Parameters.AddWithValue("$due", task.Due.Value.ToUnixTime());

                    task.Id = cmd.ExecuteScalar().ToString();

                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    tnx.Commit();
                }
            }

            return Task.FromResult(task);
        }

        public Task<UserTask[]> GetUserTasksAsync(bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserTask> GetByIdAsync(string id, bool includeDeletedRecords,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserTask> UpdateAsync(UserTask entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private void EnsureUserId()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new ArgumentException($"{nameof(UserName)} should be provided.");
        }

        private string GetUserId(string userName)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM user WHERE name = $name";
                cmd.Parameters.AddWithValue("$name", userName.ToLower());
                return cmd.ExecuteScalar()?.ToString()
                       ?? throw new EntityNotFoundException(nameof(userName), userName);
            }
        }
    }
}