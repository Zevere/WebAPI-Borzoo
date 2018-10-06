using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Data.Mongo.Tests.Shared;
using MongoDB.Bson;
using Framework;
using Xunit;

// ReSharper disable once CheckNamespace
namespace MongoTests
{
    public class UserRepoTests : IClassFixture<UserRepoTests.Fixture>
    {
        private readonly Fixture _fixture;

        public UserRepoTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact(DisplayName = "Should create a new user")]
        public async Task Should_Add_New_User()
        {
            IUserRepository repo = new UserRepository(
                _fixture.Database.GetCollection<User>("users")
            );

            User user = new User
            {
                FirstName = "Charles",
                DisplayId = "chuck",
                PassphraseHash = "secret_passphrase"
            };

            User newUser = await repo.AddAsync(user);

            Assert.Same(user, newUser);

            Assert.True(ObjectId.TryParse(newUser.Id, out _), "ID should be assigned by Mongo");

            Assert.Equal(17, newUser.PassphraseHash.Length);
            Assert.NotEmpty(newUser.DisplayId);
            Assert.Null(newUser.LastName);
            Assert.False(newUser.IsDeleted);
            Assert.Null(newUser.ModifiedAt);

            _fixture.UserId = newUser.Id;
        }

        [OrderedFact(DisplayName = "Should set token for the user")]
        public async Task Should_Set_User_Token()
        {
            string userId = _fixture.UserId;

            IUserRepository repo = new UserRepository(
                _fixture.Database.GetCollection<User>("users")
            );

            await repo.SetTokenForUserAsync(userId, "~~Token~~");
        }

        [OrderedFact(DisplayName = "Should get the user by his username")]
        public async Task Should_User_Get_By_Name()
        {
            string userId = _fixture.UserId;

            IUserRepository repo = new UserRepository(
                _fixture.Database.GetCollection<User>("users")
            );

            User user = await repo.GetByNameAsync("chuck");

            Assert.Same(user, user);
            Assert.Equal(userId, user.Id);
            Assert.Equal("Charles", user.FirstName);
            Assert.Equal("chuck", user.DisplayId);
            Assert.Equal("secret_passphrase", user.PassphraseHash);
            Assert.Equal("~~Token~~", user.Token);

            Assert.Null(user.LastName);
            Assert.False(user.IsDeleted);
            Assert.Null(user.ModifiedAt);
        }

        public class Fixture : DatabaseFixture
        {
            public string UserId { get; set; }
        }
    }
}