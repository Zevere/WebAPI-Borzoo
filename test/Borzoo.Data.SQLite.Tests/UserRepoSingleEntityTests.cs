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
                DisplayId = newDisplayId,
                FirstName = newFName,
                LastName = newLName,
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

            _fixture.NewUser.DisplayId = updatedEntity.DisplayId;
            _fixture.NewUser.PassphraseHash = updatedEntity.PassphraseHash;
            _fixture.NewUser.FirstName = updatedEntity.FirstName;
            _fixture.NewUser.LastName = updatedEntity.LastName;
            _fixture.NewUser.ModifiedAt = updatedEntity.ModifiedAt;
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

        [Fact]
        public void _6_Should_Create_And_Update_Login_For_User()
        {
            const string token = "test-token";
            const string token2 = "another-test-token";

            IUserRepository sut = new UserRepository(Connection);

            sut.SetTokenForUserAsync(_fixture.NewUser.Id, token);
            sut.SetTokenForUserAsync(_fixture.NewUser.Id, token2);

            _fixture.NewUser.Token = token2;
        }

        [Fact]
        public void _7_Should_Get_User_By_Token()
        {
            string token = _fixture.NewUser.Token;

            IUserRepository sut = new UserRepository(Connection);

            User user = sut.GetByTokenAsync(token).Result;

            Assert.Equal(_fixture.NewUser.Id, user.Id);
            Assert.Equal(_fixture.NewUser.DisplayId, user.DisplayId);
            Assert.Equal(_fixture.NewUser.PassphraseHash, user.PassphraseHash);
            Assert.Equal(_fixture.NewUser.FirstName, user.FirstName);
            Assert.Equal(_fixture.NewUser.LastName, user.LastName);
            Assert.Equal(_fixture.NewUser.Token, user.Token);
            Assert.InRange(user.JoinedAt.Ticks,
                _fixture.NewUser.JoinedAt.Ticks - 100_000,
                _fixture.NewUser.JoinedAt.Ticks + 100_000);
            if (user.ModifiedAt is null)
            {
                Assert.Equal(_fixture.NewUser.ModifiedAt, user.ModifiedAt);
            }
            else
            {
                // ReSharper disable once PossibleInvalidOperationException
                Assert.InRange(user.ModifiedAt.Value.Ticks,
                    _fixture.NewUser.ModifiedAt.Value.Ticks - 100_000,
                    _fixture.NewUser.ModifiedAt.Value.Ticks + 100_000);
            }
            Assert.Equal(_fixture.NewUser.IsDeleted, user.IsDeleted);
        }

        [Fact]
        public void _8_Should_Throw_While_Get_User_With_Non_Existing_Token()
        {
            const string token = "non-existing-token";

            IUserRepository sut = new UserRepository(Connection);
            Exception exception = Assert.ThrowsAny<Exception>(() =>
                sut.GetByTokenAsync(token).Result
            );

            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Contains("Token", exception.Message);
            Assert.Contains(token, exception.Message);
        }
    }
}