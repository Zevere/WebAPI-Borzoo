using System;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Borzoo.Data.SQLite.Tests
{
    public class UserRepoTests : IClassFixture<UserRepoFixture>
    {
        private SqliteConnection Connection => _fixture.Connection;

        private readonly UserRepoFixture _fixture;

        public UserRepoTests(UserRepoFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void _0_Should_Add_User()
        {
            User user = new User
            {
                FirstName = "Alice",
                DisplayId = "alice0",
                PassphraseHash = "secret_passphrase"
            };

            IEntityRepository<User> sut = new UserRepository(Connection);
            User entity = sut.AddAsync(user).Result;

            Assert.Same(user, entity);
            Assert.Equal(1.ToString(), entity.Id);
            Assert.Equal(17, entity.PassphraseHash.Length);
            Assert.NotEmpty(entity.DisplayId);
            Assert.Null(entity.LastName);
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.ModifiedAt);

            _fixture.NewUser = entity;
        }

        [Fact]
        public void _1_Should_Get_User_By_Id()
        {
            IEntityRepository<User> sut = new UserRepository(Connection);
            User entity = sut.GetByIdAsync(_fixture.NewUser.Id).Result;

            Assert.Equal(_fixture.NewUser.Id, entity.Id);
            Assert.Equal(_fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(_fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(_fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(_fixture.NewUser.LastName, entity.LastName);
            Assert.InRange(entity.JoinedAt.Ticks,
                _fixture.NewUser.JoinedAt.Ticks - 100_000,
                _fixture.NewUser.JoinedAt.Ticks + 100_000);
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [Fact]
        public void _2_Should_Get_User_By_Name()
        {
            string username = _fixture.NewUser.DisplayId.ToUpper();

            IUserRepository sut = new UserRepository(Connection);
            User entity = sut.GetByNameAsync(username).Result;

            Assert.Equal(_fixture.NewUser.Id, entity.Id);
            Assert.Equal(_fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(_fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(_fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(_fixture.NewUser.LastName, entity.LastName);
            Assert.InRange(entity.JoinedAt.Ticks,
                _fixture.NewUser.JoinedAt.Ticks - 100_000,
                _fixture.NewUser.JoinedAt.Ticks + 100_000);
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [Fact]
        public void _3_Should_Throw_While_Getting_Non_Existing_User()
        {
            const string id = "non-existing-id";
            IEntityRepository<User> sut = new UserRepository(Connection);

            Exception exception = Assert.ThrowsAny<Exception>(() =>
                sut.GetByIdAsync(id).Result
            );

            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Contains(id, exception.Message);
        }

        [Fact]
        public void _4_Should_Update_User()
        {
            const string newFName = "Bob";
            const string newLName = "Baker";
            const string newDisplayId = "bobby2";
            const string newPassphraseHash = "secret_passphrase2";

            User user = new User
            {
                Id = _fixture.NewUser.Id,
                FirstName = newFName,
                LastName = newLName,
                DisplayId = newDisplayId,
                PassphraseHash = newPassphraseHash,
                JoinedAt = _fixture.NewUser.JoinedAt
            };
            DateTime timeBeforeTestAction = DateTime.UtcNow;

            IEntityRepository<User> sut = new UserRepository(Connection);
            User updatedEntity = sut.UpdateAsync(user).Result;

            Assert.Same(user, updatedEntity);
            Assert.Equal(_fixture.NewUser.Id, updatedEntity.Id);
            Assert.Equal(newFName, updatedEntity.FirstName);
            Assert.Equal(newLName, updatedEntity.LastName);
            Assert.Equal(newDisplayId, updatedEntity.DisplayId);
            Assert.Equal(newPassphraseHash, updatedEntity.PassphraseHash);
            Assert.NotNull(updatedEntity.ModifiedAt);
            Assert.True(updatedEntity.ModifiedAt > timeBeforeTestAction);
            Assert.Equal(_fixture.NewUser.JoinedAt, updatedEntity.JoinedAt);
        }

        [Fact]
        public void _5_Should_Not_Override_Modified_Date()
        {
            DateTime modificationDate = DateTime.Today.AddDays(-1);

            User user = _fixture.NewUser;
            user.ModifiedAt = modificationDate;

            IEntityRepository<User> sut = new UserRepository(Connection);
            User updatedEntity = sut.UpdateAsync(user).Result;

            Assert.Equal(modificationDate, updatedEntity.ModifiedAt);
        }
    }
}