using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite;
using Data.SQLite.Tests.Shared;
using Framework;
using Xunit;

// ReSharper disable once CheckNamespace
namespace SQLiteTests
{
    public class UserRepoTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public UserRepoTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact(DisplayName = "Should create a new user")]
        public async Task Should_Add_New_User()
        {
            IUserRepository repo = new UserRepository(
                _fixture.ConnectionString
            );

            User user = new User
            {
                FirstName = "Charles",
                DisplayId = "chuck",
                PassphraseHash = "secret_passphrase",
            };

            User entity = await repo.AddAsync(user);

            Assert.Same(user, entity);
            Assert.Equal("1", entity.Id);
            Assert.Equal("Charles", entity.FirstName);
            Assert.Equal("chuck", entity.DisplayId);
            Assert.Equal("secret_passphrase", entity.PassphraseHash);

            Assert.Null(entity.LastName);
            Assert.Null(entity.Token);
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.ModifiedAt);
        }

        [OrderedFact(DisplayName = "Should set token for the user")]
        public async Task Should_Set_User_Token()
        {
            IUserRepository repo = new UserRepository(
                _fixture.ConnectionString
            );

            await repo.SetTokenForUserAsync("1", "~~Token~~");
        }

        [OrderedFact(DisplayName = "Should get the user by his username")]
        public async Task Should_User_Get_By_Name()
        {
            IUserRepository repo = new UserRepository(
                _fixture.ConnectionString
            );

            User user = await repo.GetByNameAsync("chuck");

            Assert.Same(user, user);
            Assert.Equal("1", user.Id);
            Assert.Equal("Charles", user.FirstName);
            Assert.Equal("chuck", user.DisplayId);
            Assert.Equal("secret_passphrase", user.PassphraseHash);
            Assert.Equal("~~Token~~", user.Token);

            Assert.Null(user.LastName);
            Assert.False(user.IsDeleted);
            Assert.Null(user.ModifiedAt);
        }
    }
}