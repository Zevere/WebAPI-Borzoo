using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    /// <inheritdoc />
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        private FilterDefinitionBuilder<User> Filter => Builders<User>.Filter;

        /// <inheritdoc />
        public UserRepository(
            IMongoCollection<User> collection
        )
        {
            _collection = collection;
        }

        /// <inheritdoc />
        public async Task AddAsync(
            User entity,
            CancellationToken cancellationToken = default
        )
        {
            entity.DisplayId = entity.DisplayId.ToLower();

            try
            {
                await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MongoWriteException e) when (
                e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                e.WriteError.Message.Contains(" index: username ")
            )
            {
                throw new DuplicateKeyException(nameof(User.DisplayId));
            }
        }

        /// <inheritdoc />
        public async Task<User> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                throw new EntityNotFoundException(id);
            }

            User entity = await _collection
                .Find(Filter.Eq("_id", objectId))
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                throw new EntityNotFoundException(id);
            }

            return entity;
        }

        /// <inheritdoc />
        public async Task<User> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        )
        {
            name = Regex.Escape(name);
            var filter = Filter.Regex(u => u.DisplayId, new BsonRegularExpression($"^{name}$", "i"));

            User entity = await _collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                throw new EntityNotFoundException("User Name", name);
            }

            return entity;
        }

        /// <inheritdoc />
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
                throw new EntityNotFoundException(nameof(User.Id));
            }
        }

        /// <inheritdoc />
        public async Task<User> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Builders<User>.Filter.Eq(u => u.Token, token);
            User entity = await _collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                throw new EntityNotFoundException("token", token);
            }

            return entity;
        }

        /// <inheritdoc />
        public async Task SetTokenForUserAsync(
            string userId,
            string token,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Token, token);
            await _collection.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
