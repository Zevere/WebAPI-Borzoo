using System;
using System.Linq;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite;
using Borzoo.Tests.Framework;
using SQLiteTests.Framework;
using Xunit;

namespace Borzoo.Data.Tests.SQLite.Single_Entity
{
    public class UserRepoSingleEntityTests : IClassFixture<UserRepoSingleEntityTests.Fixture>
    {
        private readonly Fixture _fixture;

        public UserRepoSingleEntityTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact]
        public async Task Should_Add_User()
        {
            IEntityRepository<User> repo = new UserRepository(_fixture.Connection);

            User user = new User
            {
                FirstName = "Alice",
                DisplayId = "alice0",
                PassphraseHash = "secret_passphrase"
            };

            var entity = await repo.AddAsync(user);

            Assert.Same(user, entity);
            Assert.Equal("1", entity.Id);
            Assert.Equal(17, entity.PassphraseHash.Length);
            Assert.NotEmpty(entity.DisplayId);
            Assert.Null(entity.LastName);
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.ModifiedAt);

            _fixture.NewUser = entity;
        }


        [OrderedFact]
        public async Task Should_Throw_While_Adding_User_Duplicate_Name()
        {
            IEntityRepository<User> repo = new UserRepository(_fixture.Connection);

            User user = new User
            {
                FirstName = "Al",
                DisplayId = _fixture.NewUser.DisplayId,
                PassphraseHash = "a new passphrase"
            };

            var e = await Assert.ThrowsAsync<DuplicateKeyException>(() =>
                repo.AddAsync(user)
            );

            Assert.Equal("DisplayId", e.Keys.Single());
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Id()
        {
            IEntityRepository<User> repo = new UserRepository(_fixture.Connection);
            User entity = await repo.GetByIdAsync(_fixture.NewUser.Id);

            Assert.Equal(_fixture.NewUser.Id, entity.Id);
            Assert.Equal(_fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(_fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(_fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(_fixture.NewUser.LastName, entity.LastName);
            Assert.Equal(entity.JoinedAt, _fixture.NewUser.JoinedAt, new DateTimeEqualityComparer());
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Name()
        {
            string username = _fixture.NewUser.DisplayId.ToUpper();

            IUserRepository sut = new UserRepository(_fixture.Connection);
            User entity = await sut.GetByNameAsync(username);

            Assert.Equal(_fixture.NewUser.Id, entity.Id);
            Assert.Equal(_fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(_fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(_fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(_fixture.NewUser.LastName, entity.LastName);
            Assert.Equal(entity.JoinedAt, _fixture.NewUser.JoinedAt, new DateTimeEqualityComparer());
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [OrderedFact]
        public async Task Should_Throw_While_Getting_Non_Existing_User()
        {
            const string id = "non-existing-id";
            IEntityRepository<User> sut = new UserRepository(_fixture.Connection);

            Exception exception = await Assert.ThrowsAnyAsync<Exception>(() =>
                sut.GetByIdAsync(id)
            );

            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Contains(id, exception.Message);
        }

        [OrderedFact]
        public async Task Should_Update_User()
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

            IEntityRepository<User> sut = new UserRepository(_fixture.Connection);
            User updatedEntity = await sut.UpdateAsync(user);

            Assert.Same(user, updatedEntity);
            Assert.Equal(_fixture.NewUser.Id, updatedEntity.Id);
            Assert.Equal(newFName, updatedEntity.FirstName);
            Assert.Equal(newLName, updatedEntity.LastName);
            Assert.Equal(newDisplayId, updatedEntity.DisplayId);
            Assert.Equal(newPassphraseHash, updatedEntity.PassphraseHash);
            Assert.NotNull(updatedEntity.ModifiedAt);
            Assert.True(updatedEntity.ModifiedAt > timeBeforeTestAction);
            Assert.Equal(updatedEntity.JoinedAt, _fixture.NewUser.JoinedAt, new DateTimeEqualityComparer());

            updatedEntity.CopyTo(_fixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Not_Override_Modified_Date()
        {
            DateTime modificationDate = DateTime.Today.AddDays(-1);

            User user = _fixture.NewUser;
            user.ModifiedAt = modificationDate;

            IEntityRepository<User> sut = new UserRepository(_fixture.Connection);
            User updatedEntity = await sut.UpdateAsync(user);

            Assert.Equal(modificationDate.ToUniversalTime(), updatedEntity.ModifiedAt,
                new NullableDateTimeEqualityComparer());

            updatedEntity.CopyTo(_fixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Set_User_Token()
        {
            const string token = "api_token";

            IUserRepository sut = new UserRepository(_fixture.Connection);

            await sut.SetTokenForUserAsync(_fixture.NewUser.Id, token);

            _fixture.NewUser.Token = token;
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Token()
        {
            IUserRepository sut = new UserRepository(_fixture.Connection);

            User entity = await sut.GetByTokenAsync(_fixture.NewUser.Token);

            Assert.Equal(_fixture.NewUser.Token, entity.Token);

            Assert.Equal(_fixture.NewUser.Id, entity.Id);
            Assert.Equal(_fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(_fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(_fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(_fixture.NewUser.LastName, entity.LastName);
            Assert.Equal(_fixture.NewUser.JoinedAt, entity.JoinedAt, new DateTimeEqualityComparer());
            Assert.NotNull(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);

            entity.CopyTo(_fixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Throw_Getting_User_By_Invalid_Token()
        {
            IUserRepository sut = new UserRepository(_fixture.Connection);

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                sut.GetByTokenAsync("invalid_token")
            );

            Assert.Contains("token", exception.Message);
        }

///*
//        [OrderedFact]
//        public void _07_Should_Get_User_By_Token()
//        {
//            string token = _fixture.NewUser.Token;
//
//            IUserRepository sut = new UserRepository(Connection);
//
//            User user = sut.GetByTokenAsync(token).Result;
//
//            Assert.Equal(_fixture.NewUser.Id, user.Id);
//            Assert.Equal(_fixture.NewUser.DisplayId, user.DisplayId);
//            Assert.Equal(_fixture.NewUser.PassphraseHash, user.PassphraseHash);
//            Assert.Equal(_fixture.NewUser.FirstName, user.FirstName);
//            Assert.Equal(_fixture.NewUser.LastName, user.LastName);
//            Assert.Equal(_fixture.NewUser.Token, user.Token);
//            Assert.InRange(user.JoinedAt.Ticks,
//                _fixture.NewUser.JoinedAt.Ticks - 100_000,
//                _fixture.NewUser.JoinedAt.Ticks + 100_000);
//            if (user.ModifiedAt is null)
//            {
//                Assert.Equal(_fixture.NewUser.ModifiedAt, user.ModifiedAt);
//            }
//            else
//            {
//                // ReSharper disable once PossibleInvalidOperationException
//                Assert.InRange(user.ModifiedAt.Value.Ticks,
//                    _fixture.NewUser.ModifiedAt.Value.Ticks - 100_000,
//                    _fixture.NewUser.ModifiedAt.Value.Ticks + 100_000);
//            }
//
//            Assert.Equal(_fixture.NewUser.IsDeleted, user.IsDeleted);
//        }
//
//        [OrderedFact]
//        public void _08_Should_Throw_While_Get_User_With_Non_Existing_Token()
//        {
//            const string token = "non-existing-token";
//
//            IUserRepository sut = new UserRepository(Connection);
//            Exception exception = Assert.ThrowsAny<Exception>(() =>
//                sut.GetByTokenAsync(token).Result
//            );
//
//            Assert.IsType<EntityNotFoundException>(exception);
//            Assert.Contains("Token", exception.Message);
//            Assert.Contains(token, exception.Message);
//        }
//
//        [OrderedFact]
//        public void _09_Should_Get_User_By_Pass_Login()
//        {
//            string userName = _fixture.NewUser.DisplayId.ToUpper();
//            string pass = _fixture.NewUser.PassphraseHash;
//
//            IUserRepository sut = new UserRepository(Connection);
//            User user = sut.GetByPassphraseLoginAsync(userName, pass).Result;
//
//            Assert.Equal(_fixture.NewUser.Id, user.Id);
//            Assert.Equal(userName, user.DisplayId);
//            Assert.Equal(pass, user.PassphraseHash);
//            Assert.Equal(_fixture.NewUser.FirstName, user.FirstName);
//            Assert.Equal(_fixture.NewUser.LastName, user.LastName);
//            Assert.Equal(_fixture.NewUser.Token, user.Token);
//            Assert.InRange(user.JoinedAt.Ticks,
//                _fixture.NewUser.JoinedAt.Ticks - 100_000,
//                _fixture.NewUser.JoinedAt.Ticks + 100_000);
//            if (user.ModifiedAt is null)
//            {
//                Assert.Equal(_fixture.NewUser.ModifiedAt, user.ModifiedAt);
//            }
//            else
//            {
//                // ReSharper disable once PossibleInvalidOperationException
//                Assert.InRange(user.ModifiedAt.Value.Ticks,
//                    _fixture.NewUser.ModifiedAt.Value.Ticks - 100_000,
//                    _fixture.NewUser.ModifiedAt.Value.Ticks + 100_000);
//            }
//
//            Assert.Equal(_fixture.NewUser.IsDeleted, user.IsDeleted);
//        }
//
//        [OrderedFact]
//        public void _10_Should_Throw_While_Get_User_By_Wrong_Name()
//        {
//            const string userName = "non-existing-user-name";
//
//            IUserRepository sut = new UserRepository(Connection);
//            Exception exception = Assert.ThrowsAny<Exception>(() =>
//                sut.GetByPassphraseLoginAsync(userName, "secret-passpharse").Result
//            );
//
//            Assert.IsType<EntityNotFoundException>(exception);
//            Assert.Contains("Name", exception.Message);
//            Assert.Contains(userName, exception.Message);
//        }
//
//        [OrderedFact]
//        public void _11_Should_Throw_While_Get_User_By_Wrong_Pass()
//        {
//            string userName = _fixture.NewUser.DisplayId;
//
//            IUserRepository sut = new UserRepository(Connection);
//            Exception exception = Assert.ThrowsAny<Exception>(() =>
//                sut.GetByPassphraseLoginAsync(userName, "wrong-secret-passpharse").Result
//            );
//
//            Assert.IsType<EntityNotFoundException>(exception);
//            Assert.Contains("Name", exception.Message);
//            Assert.Contains(userName, exception.Message);
//        }
//
//        [OrderedFact]
//        public void _12_Should_Remove_User_Login()
//        {
//            string token = _fixture.NewUser.Token;
//
//            IUserRepository sut = new UserRepository(Connection);
//
//            bool isRevoked = sut.RevokeTokenAsync(token).Result;
//            bool isRevoked2 = sut.RevokeTokenAsync(token).Result;
//
//            Assert.True(isRevoked);
//            Assert.False(isRevoked2);
//        }
//
//        [OrderedFact]
//        public void _13_Should_Delete_User()
//        {
//            string userId = _fixture.NewUser.Id;
//
//            IUserRepository sut = new UserRepository(Connection);
//
//            sut.DeleteAsync(userId).Wait();
//        }
//
//        [OrderedFact]
//        public void _14_Should_Get_SoftDeleted_User()
//        {
//            string userId = _fixture.NewUser.Id;
//
//            IUserRepository sut = new UserRepository(Connection);
//
//            var user = sut.GetByIdAsync(userId, true).Result;
//
//            Assert.Equal(userId, user.Id);
//            Assert.True(user.IsDeleted);
//        }
//
//        [OrderedFact]
//        public void _15_Should_Throw_While_Deleting_NonExisting_User()
//        {
//            const string id = "non-existing-user";
//            IUserRepository sut = new UserRepository(Connection);
//
//            var exception = Assert.Throws<EntityNotFoundException>(
//                () => sut.DeleteAsync(id).Wait()
//            );
//
//            Assert.Contains(id, exception.Message);
//        }
//*/
        public class Fixture : FixtureBase
        {
            public User NewUser { get; set; }

            public Fixture()
                : base(nameof(UserRepoSingleEntityTests))
            {
            }
        }
    }
}