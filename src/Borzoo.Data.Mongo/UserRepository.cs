using System;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoCollection<User> collection)
        {
            _collection = collection;
        }

        public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await _collection.InsertOneAsync(entity, default, cancellationToken);
            }
            catch (MongoWriteException e)
                when (e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                      e.WriteError.Message.Contains($" index: {MongoConstants.Collections.Users.Indexes.Username} "))
            {
                throw new DuplicateKeyException("name");
            }

            return entity;
        }

        public Task<User> GetByIdAsync(string id, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByTokenAsync(string token, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByPassphraseLoginAsync(string userName, string passphrase,
            bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SetTokenForUserAsync(string userId, string token, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}