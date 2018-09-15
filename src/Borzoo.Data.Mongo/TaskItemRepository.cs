using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo.Entities;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public class TaskItemRepository : ITaskItemRepository
    {
        public string TaskListName { get; private set; }

        public string UserName { get; private set; }

        public string TaskListId { get; private set; }

        public string UserId { get; private set; }

        private readonly IMongoCollection<TaskItemMongo> _collection;

        private readonly ITaskListRepository _tasklistRepo;

        public TaskItemRepository(IMongoCollection<TaskItemMongo> collection, ITaskListRepository tasklistRepo)
        {
            _collection = collection;
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
            entity.DisplayId = entity.DisplayId.ToLower();

            var taskMongo = TaskItemMongo.FromTaskItem(entity);
            taskMongo.ListDbRef = new MongoDBRef(MongoConstants.Collections.TaskLists.Name, TaskListId);
            try
            {
                await _collection.InsertOneAsync(taskMongo, cancellationToken: cancellationToken);
            }
            catch (MongoWriteException e)
                when (e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                      e.WriteError.Message
                          .Contains($" index: {MongoConstants.Collections.TaskItems.Indexes.ListTaskName} ")
                )
            {
                throw new DuplicateKeyException(nameof(TaskItem.ListId), nameof(TaskItem.DisplayId));
            }

            entity.Id = taskMongo.Id;
            entity.ListId = taskMongo.ListDbRef.Id.AsString;
            return entity;
        }

        public Task<TaskItem> GetByIdAsync(string id, bool includeDeletedRecords = false,
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

        public Task<TaskItem> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskItem[]> GetTaskItemsAsync(bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            EnsureListId();

            var filter = Builders<TaskItemMongo>
                .Filter.Eq(task => task.ListDbRef.Id, TaskListId);

            var taskitems = await _collection
                .Find(filter)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var array = taskitems
                .Cast<TaskItem>()
                .ToArray();

            return array;
        }

        private void EnsureListId()
        {
            if (string.IsNullOrWhiteSpace(TaskListId))
                throw new ArgumentException($"{nameof(TaskListName)} should be provided.");
        }
    }
}