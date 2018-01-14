using System;
using System.Collections.Generic;
using System.Data;
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

            string sql = "INSERT INTO task(user_id, name, title, created_at" +
                         (hasDescription ? ", description" : string.Empty) +
                         (hasDueDate ? ", due" : string.Empty) +
                         ")" +
                         "VALUES ($user_id, $name, $title, $created_at" +
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
                    cmd.Parameters.AddWithValue("$name", task.Name.ToLower());
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

        public Task<UserTask> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            EnsureUserId();
            string sql = "SELECT id, title, description, due, created_at, modified_at, is_deleted " +
                         "FROM task " +
                         "WHERE LOWER(name) = LOWER($name) AND user_id = $user_id ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            UserTask entity;
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$name", name);
                cmd.Parameters.AddWithValue("$user_id", UserId);
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!(reader.HasRows && reader.Read()))
                    {
                        throw new EntityNotFoundException(nameof(name), name);
                    }

                    entity = new UserTask
                    {
                        Id = reader["id"].ToString(),
                        Name = name.ToLower(),
                        Title = reader["title"].ToString(),
                        Description = reader["description"].ToString(),
                        CreatedAt = long.Parse(reader["created_at"].ToString()).FromUnixTime(),
                        IsDeleted = !string.IsNullOrWhiteSpace(reader["is_deleted"].ToString())
                    };
                    
                    if (string.IsNullOrWhiteSpace(entity.Description))
                        entity.Description = default;
                    
                    string dueValue = reader["due"].ToString();
                    if (!string.IsNullOrWhiteSpace(dueValue))
                        entity.Due = long.Parse(dueValue).FromUnixTime();

                    string modifiedAtValue = reader["modified_at"].ToString();
                    if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                        entity.ModifiedAt = long.Parse(modifiedAtValue).FromUnixTime();
                }
            }

            return Task.FromResult(entity);
        }

        public Task<UserTask[]> GetUserTasksAsync(bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            EnsureUserId();
            string sql = "SELECT id, name, title, description, due, created_at, modified_at, is_deleted " +
                         "FROM task " +
                         "WHERE user_id = $user_id ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            UserTask[] tasks;
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$user_id", UserId);
                using (var reader = cmd.ExecuteReader())
                {
                    var list = new List<UserTask>();
                    while (reader.Read())
                    {
                        var t = new UserTask
                        {
                            Id = reader["id"].ToString(),
                            Name = reader["name"].ToString(),
                            Title = reader["title"].ToString(),
                            Description = reader["description"].ToString(),
                            CreatedAt = long.Parse(reader["created_at"].ToString()).FromUnixTime(),
                            IsDeleted = !string.IsNullOrWhiteSpace(reader["is_deleted"].ToString())
                        };

                        if (string.IsNullOrWhiteSpace(t.Description))
                            t.Description = default;

                        string dueValue = reader["due"].ToString();
                        if (!string.IsNullOrWhiteSpace(dueValue))
                            t.Due = long.Parse(dueValue).FromUnixTime();

                        string modifiedAtValue = reader["modified_at"].ToString();
                        if (!string.IsNullOrWhiteSpace(modifiedAtValue))
                            t.ModifiedAt = long.Parse(modifiedAtValue).FromUnixTime();

                        list.Add(t);
                    }

                    tasks = list.ToArray();
                }
            }

            return Task.FromResult(tasks);
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