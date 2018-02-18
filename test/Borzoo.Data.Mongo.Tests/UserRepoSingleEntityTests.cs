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
        
        [OrderedFact]
        public async Task Should_Get_User_By_Id()
        {
            IEntityRepository<User> sut = new UserRepository(UsersCollection);
            User entity = await sut.GetByIdAsync(_fixture.NewUser.Id);

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