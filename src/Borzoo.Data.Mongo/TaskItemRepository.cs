using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private FilterDefinitionBuilder<TaskItem> Filter => Builders<TaskItem>.Filter;

        private readonly IMongoCollection<TaskItem> _collection;

        public TaskItemRepository(
            IMongoCollection<TaskItem> collection
        )
        {
            _collection = collection;
        }

        public async Task AddAsync(
            TaskItem entity,
            CancellationToken cancellationToken = default
        )
        {
            entity.OwnerId = entity.OwnerId.ToLower();
            entity.ListId = entity.ListId.ToLower();
            entity.DisplayId = entity.DisplayId.ToLower();

            try
            {
                await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MongoWriteException e) when (
                e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                e.WriteError.Message.Contains(" index: owner_list_task-name ")
            )
            {
                throw new DuplicateKeyException(
                    nameof(TaskItem.OwnerId), nameof(TaskItem.ListId), nameof(TaskItem.DisplayId)
                );
            }
        }

        public Task<TaskItem> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task<TaskItem> UpdateAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _collection
                .DeleteOneAsync(Filter.Eq("_id", ObjectId.Parse(id)), cancellationToken)
                .ConfigureAwait(false);

            if (result.DeletedCount == 0)
            {
                throw new EntityNotFoundException(nameof(TaskItem.Id));
            }
        }

        public async Task<TaskItem> GetByNameAsync(
            string name,
            string ownerName,
            string listName,
            CancellationToken cancellationToken = default
        )

        {
            name = Regex.Escape(name);
            ownerName = Regex.Escape(ownerName);
            listName = Regex.Escape(listName);

            var filter = Filter.And(
                Filter.Regex(t => t.DisplayId, new BsonRegularExpression($"^{name}$", "i")),
                Filter.Regex(t => t.OwnerId, new BsonRegularExpression($"^{ownerName}$", "i")),
                Filter.Regex(t => t.ListId, new BsonRegularExpression($"^{listName}$", "i"))
            );

            var taskItem = await _collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (taskItem is null)
            {
                throw new EntityNotFoundException(nameof(TaskItem.DisplayId), name);
            }

            return taskItem;
        }

        public async Task<TaskItem[]> GetTaskItemsForListAsync(
            string ownerName,
            string listName,
            CancellationToken cancellationToken = default
        )
        {
            ownerName = Regex.Escape(ownerName);
            listName = Regex.Escape(listName);

            var filter = Filter.And(
                Filter.Regex(t => t.OwnerId, new BsonRegularExpression($"^{ownerName}$", "i")),
                Filter.Regex(t => t.ListId, new BsonRegularExpression($"^{listName}$", "i"))
            );

            var taskItems = await _collection
                .Find(filter)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return taskItems.ToArray();
        }
    }
}
