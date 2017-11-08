using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Xunit;

namespace Borzoo.Data.SQLite.Tests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task Should_Add_User()
        {
            User user = new User
            {
                FirstName = "Alice",
                DisplayId = "alice0",
                PassphraseHash = "secret_passphrase"
            };

            User entity;
            using (var connection = Helpers.CreateInMemoryDatabase(nameof(Should_Add_User)))
            {
                IEntityRepository<User> sut = new UserRepository(connection);
                entity = await sut.AddAsync(user);
            }

            Assert.Same(user, entity);
            Assert.Equal(1.ToString(), entity.Id);
            Assert.NotEmpty(entity.DisplayId);
            Assert.Null(entity.LastName);
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.ModifiedAt);
        }
    }
}