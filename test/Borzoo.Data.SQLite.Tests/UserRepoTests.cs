using System.Threading.Tasks;
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
        public async Task _0_Should_Add_User()
        {
            User user = new User
            {
                FirstName = "Alice",
                DisplayId = "alice0",
                PassphraseHash = "secret_passphrase"
            };

            IEntityRepository<User> sut = new UserRepository(Connection);
            User entity = await sut.AddAsync(user);

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
        public async Task _1_Should_Get_User_By_Id()
        {
            IEntityRepository<User> sut = new UserRepository(Connection);
            User entity = await sut.GetAsync(_fixture.NewUser.Id);

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
    }
}