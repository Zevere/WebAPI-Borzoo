using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Borzoo.Data.SQLite
{
    public class TaskItemRepository : EntityRepositoryBase, ITaskItemRepository
    {
        public string TaskListName { get; private set; }

        public string UserName { get; private set; }

        public string TaskListId { get; private set; }

        public string UserId { get; private set; }

        private readonly ITaskListRepository _tasklistRepo;

        public TaskItemRepository(string connectionString, ITaskListRepository tasklistRepo)
            : base(connectionString)
        {
            _tasklistRepo = tasklistRepo;
        }

        public TaskItemRepository(SqliteConnection connection, ITaskListRepository tasklistRepo)
            : base(connection)
        {
            _tasklistRepo = tasklistRepo;
        }

        public async Task SetTaskListAsync(string username, string tasklistName,
            CancellationToken cancellationToken = default)
        {
            await _tasklistRepo.SetUsernameAsync(username, cancellationToken)
                .ConfigureAwait(false);
            var tasklist = await _tasklistRepo.GetByNameAsync(tasklistName, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            UserId = tasklist.OwnerId;
            UserName = username;
            TaskListId = tasklist.Id;
            TaskListName = tasklist.DisplayId;
        }

        public async Task<TaskItem> AddAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            EnsureListId();
            bool hasDescription = !string.IsNullOrWhiteSpace(entity.Description);
            bool hasDueDate = entity.Due != default;

            string sql = "INSERT INTO task(list_id, name, title, created_at" +
                         (hasDescription ? ", description" : string.Empty) +
                         (hasDueDate ? ", due" : string.Empty) +
                         ")" +
                         "VALUES ($list_id, $name, $title, $created_at" +
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
                    cmd.Parameters.AddWithValue("$list_id", TaskListId);
                    cmd.Parameters.AddWithValue("$name", entity.DisplayId.ToLower());
                    cmd.Parameters.AddWithValue("$title", entity.Title);
                    cmd.Parameters.AddWithValue("$created_at", entity.CreatedAt.ToUnixTime());
                    if (hasDescription)
                        cmd.Parameters.AddWithValue("$description", entity.Description);
                    if (hasDueDate)
                        cmd.Parameters.AddWithValue("$due", entity.Due.Value.ToUnixTime());

                    try
                    {
                        var result = await cmd.ExecuteScalarAsync(cancellationToken)
                            .ConfigureAwait(false);
                        entity.Id = result.ToString();
                        entity.ListId = TaskListId;
                    }
                    catch (SqliteException e)
                        when (e.SqliteErrorCode == raw.SQLITE_CONSTRAINT &&
                              e.Message.Contains("task.list_id, task.name"))
                    {
                        throw new DuplicateKeyException(nameof(TaskItem.ListId), nameof(TaskItem.DisplayId));
                    }

                    tnx.Commit();
                }
            }

            return entity;
        }

        public Task<TaskItem> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            EnsureListId();
            string sql = "SELECT id, title, description, due, created_at, modified_at, is_deleted " +
                         "FROM task " +
                         "WHERE LOWER(name) = LOWER($name) AND user_id = $user_id ";

            if (!includeDeletedRecords)
            {
                sql += "AND is_deleted IS NULL";
            }

            TaskItem entity;
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$name", name);
                cmd.Parameters.AddWithValue("$user_id", TaskListId);
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!(reader.HasRows && reader.Read()))
                    {
                        throw new EntityNotFoundException(nameof(name), name);
                    }

                    entity = new TaskItem
                    {
                        Id = reader["id"].ToString(),
                        DisplayId = name.ToLower(),
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

        public async Task<TaskItem[]> GetTaskItemsAsync(bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            EnsureListId();
            string sql = "SELECT id, name, title, description, due, created_at, modified_at, is_deleted " +
                         "FROM task " +
                         "WHERE list_id = $list_id ";

            if (!includeDeletedRecords)
                sql += "AND is_deleted IS NULL";

            TaskItem[] tasksItem;
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$list_id", TaskListId);
                var reader = await cmd.ExecuteReaderAsync(cancellationToken)
                    .ConfigureAwait(false);
                using (reader)
                {
                    var list = new List<TaskItem>();
                    while (await reader.ReadAsync(cancellationToken)
                        .ConfigureAwait(false)
                    )
                    {
                        var t = new TaskItem
                        {
                            Id = reader["id"].ToString(),
                            DisplayId = reader["name"].ToString(),
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

                    tasksItem = list.ToArray();
                }
            }

            return tasksItem;
        }

        public Task<TaskItem> GetByIdAsync(string id, bool includeDeletedRecords,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TaskItem> UpdateAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private void EnsureListId()
        {
            if (string.IsNullOrWhiteSpace(TaskListId))
                throw new ArgumentException($"{nameof(TaskListName)} should be provided.");
        }
    }
}