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
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        private FilterDefinitionBuilder<User> Filter => Builders<User>.Filter;

        public UserRepository(IMongoCollection<User> collection)
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
            catch (MongoWriteException e)
                when (e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                      e.WriteError.Message.Contains($" index: {MongoConstants.Collections.Users.Indexes.Username} "))
            {
                throw new DuplicateKeyException(nameof(User.DisplayId));
            }
        }

        public async Task<User> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            if (!ObjectId.TryParse(id, out _))
            {
                throw new EntityNotFoundException(id);
            }

            User entity = await _collection
                .Find(Filter.Eq(u => u.Id, id.ToLower()))
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                throw new EntityNotFoundException(id);
            }

            return entity;
        }

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

        public async Task<User> UpdateAsync(
            User entity,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, entity.Id);
            var updates = Builders<User>.Update.Combine(
                Builders<User>.Update.Set(u => u.DisplayId, entity.DisplayId.ToLower()),
                Builders<User>.Update.Set(u => u.PassphraseHash, entity.PassphraseHash),
                Builders<User>.Update.Set(u => u.FirstName, entity.FirstName),
                Builders<User>.Update.Set(u => u.ModifiedAt, entity.ModifiedAt?.ToUniversalTime() ?? DateTime.UtcNow),
                Builders<User>.Update.Set(u => u.LastName, entity.LastName)
            );

            var updatedEntity = await _collection.FindOneAndUpdateAsync(filter, updates,
                new FindOneAndUpdateOptions<User>
                {
                    ReturnDocument = ReturnDocument.After
                }, cancellationToken);

            updatedEntity.CopyTo(entity);

            return entity;
        }

        public Task DeleteAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Builders<User>.Filter.Eq(u => u.Token, token);
            User entity = await _collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

            if (entity is null)
            {
                throw new EntityNotFoundException("token", token);
            }

            return entity;
        }

        public Task<User> GetByPassphraseLoginAsync(
            string userName,
            string passphrase,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

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

        public Task<bool> RevokeTokenAsync(
            string token,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }
    }
}
