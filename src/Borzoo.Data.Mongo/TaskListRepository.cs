using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public class TaskListRepository : ITaskListRepository
    {
        public string UserName { get; private set; }

        public string UserId { get; private set; }

        private FilterDefinitionBuilder<TaskListMongo> Filter => Builders<TaskListMongo>.Filter;

        private readonly IMongoCollection<TaskListMongo> _collection;

        private readonly IUserRepository _userRepo;

        public TaskListRepository(IMongoCollection<TaskListMongo> collection, IUserRepository userRepo)
        {
            _collection = collection;
            _userRepo = userRepo;
        }

        public async Task SetUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByNameAsync(username, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            UserName = user.DisplayId;
            UserId = user.Id;
        }

        public async Task<TaskList> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            EnsureUserId();

            name = Regex.Unescape(name).ToLower();

            var filter = Filter.And(
                Filter.Eq(list => list.OwnerDbRef.Id, UserId),
                Filter.Regex(list => list.DisplayId, new BsonRegularExpression($"^{name}$", "i")),
                Filter.Exists(list => list.IsDeleted, includeDeletedRecords)
            );

            TaskListMongo tl;

            try
            {
                tl = await _collection
                    .Find(filter)
                    .SingleAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                throw new EntityNotFoundException(nameof(TaskList.DisplayId), name);
            }

            tl.DisplayId = name;
            return tl;
        }

        public async Task<TaskList> AddAsync(TaskList entity, CancellationToken cancellationToken = default)
        {
            EnsureUserId();
            entity.DisplayId = entity.DisplayId.ToLower();

            var tlMongo = TaskListMongo.FromTaskList(entity);
            tlMongo.OwnerDbRef = new MongoDBRef(MongoConstants.Collections.Users.Name, UserId);
            try
            {
                await _collection.InsertOneAsync(tlMongo, null, cancellationToken);
            }
            catch (MongoWriteException e)
                when (e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                      e.WriteError.Message
                          .Contains($" index: {MongoConstants.Collections.TaskLists.Indexes.OwnerListName} ")
                )
            {
                throw new DuplicateKeyException(nameof(TaskList.OwnerId), nameof(TaskList.DisplayId));
            }

            entity.OwnerId = tlMongo.OwnerDbRef.Id.AsString;
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
            var filter = Builders<TaskListMongo>
                .Filter.Eq(list => list.OwnerDbRef.Id, UserId);

            var taskLists = await _collection
                .Find(filter)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var array = taskLists
                .Cast<TaskList>()
                .ToArray();

            return array;
        }

        private void EnsureUserId()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new ArgumentNullException(nameof(UserId), $"Call {nameof(SetUsernameAsync)} method first.");
        }
    }
}