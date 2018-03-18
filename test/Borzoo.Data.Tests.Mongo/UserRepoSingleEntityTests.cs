using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Borzoo.Data.Tests.Common;
using Borzoo.Data.Tests.Mongo.Framework;
using Borzoo.Tests.Framework;
using MongoDB.Bson;
using Xunit;

namespace Borzoo.Data.Tests.Mongo
{
    public class UserRepoSingleEntityTests : UserRepoSingleEntityTestsBase,
        IClassFixture<UserRepoSingleEntityTests.Fixture>
    {
        public UserRepoSingleEntityTests(Fixture classFixture)
            : base(classFixture, () => new UserRepository(classFixture.Collection))
        {
        }

        [OrderedFact]
        public async Task Should_Add_User_Mongo()
        {
            IUserRepository repo = CreateUserRepository();

            User user = new User
            {
                FirstName = "Charles",
                DisplayId = "chuck",
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
        }

        public class Fixture : FixtureBase<User>, IUserRepoSingleEntityFixture
        {
            public User NewUser { get; set; }

            public Fixture()
                : base(MongoConstants.Collections.Users.Name)
            {
            }
        }
    }
}