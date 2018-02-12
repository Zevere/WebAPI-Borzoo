using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo.Tests.Framework;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Borzoo.Data.Mongo.Tests
{
    public class UserRepoSingleEntityTests : IClassFixture<UserRepoSingleEntityTests.Fixture>
    {
        private IMongoCollection<User> UsersCollection => _fixture.Collection;

        private readonly Fixture _fixture;

        public UserRepoSingleEntityTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact]
        public async Task Should_Add_User()
        {
            IUserRepository repo = new UserRepository(UsersCollection);

            User user = new User
            {
                FirstName = "Alice",
                DisplayId = "alice0",
                PassphraseHash = "secret_passphrase"
            };

            var entity = await repo.AddAsync(user);

            Assert.Same(user, entity);
            Assert.True(ObjectId.TryParse(entity.Id, out var _));
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
            IUserRepository repo = new UserRepository(UsersCollection);

            User user = new User
            {
                FirstName = "Al",
                DisplayId = _fixture.NewUser.DisplayId,
                PassphraseHash = "a new passphrase"
            };

            var e = await Assert.ThrowsAsync<DuplicateKeyException>(() =>
                repo.AddAsync(user)
            );

            Assert.Equal("name", e.Key);
        }

        public class Fixture : FixtureBase<User>
        {
            public User NewUser { get; set; }

            public Fixture()
                : base(MongoConstants.Collections.Users.Name)
            {
            }
        }
    }
}