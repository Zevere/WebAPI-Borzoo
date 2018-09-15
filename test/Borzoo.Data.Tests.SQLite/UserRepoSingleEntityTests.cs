using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite;
using Borzoo.Data.Tests.Common;
using Borzoo.Data.Tests.SQLite.Framework;
using Borzoo.Tests.Framework;
using Xunit;

namespace Borzoo.Data.Tests.SQLite
{
    public class UserRepoSingleEntityTests : UserRepoSingleEntityTestsBase,
        IClassFixture<UserRepoSingleEntityTests.Fixture>
    {
        public UserRepoSingleEntityTests(Fixture classFixture)
            : base(classFixture, () => new UserRepository(classFixture.Connection))
        {
        }

        [OrderedFact]
        public async Task Should_Add_User_SQLite()
        {
            User user = new User
            {
                FirstName = "Chris",
                DisplayId = "cc",
                PassphraseHash = "secret_passphrase"
            };

            IUserRepository repo = CreateUserRepository();
            User entity = await repo.AddAsync(user);

            Assert.Equal(1.ToString(), entity.Id);
        }

/*
        [OrderedFact]
        public void _07_Should_Get_User_By_Token()
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

        [OrderedFact]
        public void _08_Should_Throw_While_Get_User_With_Non_Existing_Token()
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

        [OrderedFact]
        public void _09_Should_Get_User_By_Pass_Login()
        {
            string userName = _fixture.NewUser.DisplayId.ToUpper();
            string pass = _fixture.NewUser.PassphraseHash;

            IUserRepository sut = new UserRepository(Connection);
            User user = sut.GetByPassphraseLoginAsync(userName, pass).Result;

            Assert.Equal(_fixture.NewUser.Id, user.Id);
            Assert.Equal(userName, user.DisplayId);
            Assert.Equal(pass, user.PassphraseHash);
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

        [OrderedFact]
        public void _10_Should_Throw_While_Get_User_By_Wrong_Name()
        {
            const string userName = "non-existing-user-name";

            IUserRepository sut = new UserRepository(Connection);
            Exception exception = Assert.ThrowsAny<Exception>(() =>
                sut.GetByPassphraseLoginAsync(userName, "secret-passpharse").Result
            );

            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Contains("Name", exception.Message);
            Assert.Contains(userName, exception.Message);
        }

        [OrderedFact]
        public void _11_Should_Throw_While_Get_User_By_Wrong_Pass()
        {
            string userName = _fixture.NewUser.DisplayId;

            IUserRepository sut = new UserRepository(Connection);
            Exception exception = Assert.ThrowsAny<Exception>(() =>
                sut.GetByPassphraseLoginAsync(userName, "wrong-secret-passpharse").Result
            );

            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Contains("Name", exception.Message);
            Assert.Contains(userName, exception.Message);
        }

        [OrderedFact]
        public void _12_Should_Remove_User_Login()
        {
            string token = _fixture.NewUser.Token;

            IUserRepository sut = new UserRepository(Connection);

            bool isRevoked = sut.RevokeTokenAsync(token).Result;
            bool isRevoked2 = sut.RevokeTokenAsync(token).Result;

            Assert.True(isRevoked);
            Assert.False(isRevoked2);
        }

        [OrderedFact]
        public void _13_Should_Delete_User()
        {
            string userId = _fixture.NewUser.Id;

            IUserRepository sut = new UserRepository(Connection);

            sut.DeleteAsync(userId).Wait();
        }

        [OrderedFact]
        public void _14_Should_Get_SoftDeleted_User()
        {
            string userId = _fixture.NewUser.Id;

            IUserRepository sut = new UserRepository(Connection);

            var user = sut.GetByIdAsync(userId, true).Result;

            Assert.Equal(userId, user.Id);
            Assert.True(user.IsDeleted);
        }

        [OrderedFact]
        public void _15_Should_Throw_While_Deleting_NonExisting_User()
        {
            const string id = "non-existing-user";
            IUserRepository sut = new UserRepository(Connection);

            var exception = Assert.Throws<EntityNotFoundException>(
                () => sut.DeleteAsync(id).Wait()
            );

            Assert.Contains(id, exception.Message);
        }
*/
        public class Fixture : FixtureBase, IUserRepoSingleEntityFixture
        {
            public User NewUser { get; set; }

            public Fixture()
                : base(nameof(UserRepoSingleEntityTests))
            {
            }
        }
    }
}