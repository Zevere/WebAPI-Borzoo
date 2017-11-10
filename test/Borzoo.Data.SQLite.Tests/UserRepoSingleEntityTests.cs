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
            User entity = sut.GetAsync(_fixture.NewUser.Id).Result;

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
        public void _2_Should_Throw_While_Getting_Non_Existing_User()
        {
            IEntityRepository<User> sut = new UserRepository(Connection);

            Exception exception = Assert.ThrowsAny<Exception>(() =>
                sut.GetAsync("2").Result
            );

            Assert.Equal("Not found!", exception.Message);
        }

        [Fact]
        public void _3_Should_Update_User()
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
                // ToDo: other values
            };

            IEntityRepository<User> sut = new UserRepository(Connection);
            User updatedEntity = sut.UpdateAsync(user).Result;

            Assert.Same(user, updatedEntity);
            Assert.Equal(_fixture.NewUser.Id, updatedEntity.Id);
            // ToDo: more asserts
        }
    }
}