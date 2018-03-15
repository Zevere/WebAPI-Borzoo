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
    public class TaskListRepository : EntityRepositoryBase, ITaskListRepository
    {
        public string UserName { get; private set; }

        public string UserId { get; private set; }

        private readonly IUserRepository _userRepo;

        public TaskListRepository(string connectionString, IUserRepository userRepo)
            : base(connectionString)
        {
            _userRepo = userRepo;
        }

        public TaskListRepository(SqliteConnection connection, IUserRepository userRepo)
            : base(connection)
        {
            _userRepo = userRepo;
        }

        public async Task SetUsername(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByNameAsync(username, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            UserName = user.DisplayId;
            UserId = user.Id;
        }

        public async Task<TaskList> AddAsync(TaskList entity, CancellationToken cancellationToken = default)
        {
            EnsureUserId();
            string sql = "INSERT INTO tasklist(owner_id, name, title, created_at) " +
                         "VALUES ($owner_id, $name, $title, $created_at); " +
                         "SELECT last_insert_rowid() AS id";

            using (var tnx = Connection.BeginTransaction())
            {
                var cmd = Connection.CreateCommand();
                cmd.Transaction = tnx;

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("$owner_id", UserId);
                cmd.Parameters.AddWithValue("$name", entity.DisplayId);
                cmd.Parameters.AddWithValue("$title", entity.Title);
                cmd.Parameters.AddWithValue("$created_at", entity.CreatedAt.ToUnixTime());

                try
                {
                    entity.Id = (await cmd.ExecuteScalarAsync(cancellationToken)).ToString();
                    entity.OwnerId = UserId;
                }
                catch (SqliteException e)
                    when (e.SqliteErrorCode == raw.SQLITE_CONSTRAINT &&
                          e.Message.Contains("tasklist.owner_id, tasklist.name"))
                {
                    throw new DuplicateKeyException(nameof(TaskList.OwnerId), nameof(TaskList.DisplayId));
                }

                cancellationToken.ThrowIfCancellationRequested();
                tnx.Commit();
            }

            return entity;
        }

        public Task<TaskList> GetByIdAsync(string id, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TaskList> UpdateAsync(TaskList entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskList[]> GetUserTaskListsAsync(CancellationToken cancellationToken = default)
        {
            EnsureUserId();
            string sql = "SELECT id, name, title, created_at FROM tasklist " +
                         "WHERE owner_id = $owner_id";

            var cmd = Connection.CreateCommand();

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$owner_id", UserId);

            var entities = new List<TaskList>();
            using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    var entity = new TaskList
                    {
                        Id = reader["id"].ToString(),
                        OwnerId = UserId,
                        DisplayId = reader["name"].ToString(),
                        Title = reader["title"].ToString(),
                        CreatedAt = long.Parse(reader["created_at"].ToString()).FromUnixTime(),
                    };
                    entities.Add(entity);
                }
            }

            return entities.ToArray();
        }

        private void EnsureUserId()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new ArgumentNullException(nameof(UserId), $"Call {nameof(SetUsername)} method first.");
        }
    }
}