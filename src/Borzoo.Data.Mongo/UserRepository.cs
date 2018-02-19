﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        public async Task<User> GetByIdAsync(string id, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id.ToLower());
            User entity = await _collection.Find(filter).SingleOrDefaultAsync(cancellationToken);

            if (entity is default)
            {
                throw new EntityNotFoundException(id);
            }

            return entity;
        }

        public async Task<User> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default)
        {
            // ToDo inclue deleted
            name = name.ToLower();
            User entity = await _collection
                .AsQueryable()
                .SingleOrDefaultAsync(
                    u => u.DisplayId.ToLower().Contains(name),
                    cancellationToken
                );

            if (entity is default)
            {
                throw new EntityNotFoundException("User Name", name);
            }

            return entity;
        }

        public async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
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

        public Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default)
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