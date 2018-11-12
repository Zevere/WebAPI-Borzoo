﻿using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public class TaskListRepository : ITaskListRepository
    {
        private FilterDefinitionBuilder<TaskList> Filter => Builders<TaskList>.Filter;

        private readonly IMongoCollection<TaskList> _collection;

        public TaskListRepository(
            IMongoCollection<TaskList> collection
        )
        {
            _collection = collection;
        }

        /// <inheritdoc />
        public async Task AddAsync(
            TaskList entity,
            CancellationToken cancellationToken = default
        )
        {
            entity.DisplayId = entity.DisplayId.ToLower();
            entity.OwnerId = entity.OwnerId.ToLower();

            try
            {
                await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MongoWriteException e) when (
                e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                e.WriteError.Message.Contains($" index: owner_list-name ")
            )
            {
                throw new DuplicateKeyException(nameof(TaskList.OwnerId), nameof(TaskList.DisplayId));
            }
        }

        public Task<TaskList> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task<TaskList> UpdateAsync(
            TaskList entity,
            CancellationToken cancellationToken = default
        )
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
                throw new EntityNotFoundException(nameof(TaskList.Id));
            }
        }

        /// <inheritdoc />
        public async Task<TaskList> GetByNameAsync(
            string name,
            string ownerId,
            CancellationToken cancellationToken = default
        )
        {
            name = Regex.Unescape(name);
            ownerId = Regex.Unescape(ownerId);

            var filter = Filter.And(
                Filter.Regex(tl => tl.DisplayId, new BsonRegularExpression($"^{name}$", "i")),
                Filter.Regex(tl => tl.OwnerId, new BsonRegularExpression($"^{ownerId}$", "i"))
            );

            var taskList = await _collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (taskList is null)
            {
                throw new EntityNotFoundException(nameof(TaskList.DisplayId), name);
            }

            return taskList;
        }

        public async Task<TaskList[]> GetUserTaskListsAsync(
            string ownerId,
            CancellationToken cancellationToken = default
        )
        {
            // ToDO ensure id contains allowed characters only \.[A-Z]_\d

            var filter = Filter.Regex(tl => tl.OwnerId, new BsonRegularExpression($"^{ownerId}$", "i"));

            var taskLists = await _collection
                .Find(filter)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return taskLists.ToArray();
        }

        public Task SetUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
