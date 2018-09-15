using System;
using System.Linq;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Tests.Framework;
using Xunit;

namespace Borzoo.Data.Tests.Common
{
    public abstract class UserRepoSingleEntityTestsBase
    {
        protected readonly IUserRepoSingleEntityFixture ClassFixture;

        protected Func<IUserRepository> CreateUserRepository { get; }

        protected UserRepoSingleEntityTestsBase(IUserRepoSingleEntityFixture classFixture,
            Func<IUserRepository> repoResolver)
        {
            ClassFixture = classFixture;
            CreateUserRepository = repoResolver;
        }

        [OrderedFact]
        public async Task Should_Add_User()
        {
            IEntityRepository<User> repo = CreateUserRepository();

            User user = new User
            {
                FirstName = "Alice",
                DisplayId = "alice0",
                PassphraseHash = "secret_passphrase"
            };

            var entity = await repo.AddAsync(user);

            Assert.Same(user, entity);
            Assert.NotEmpty(entity.Id); // ToDo test in specific implementations
            Assert.Equal(17, entity.PassphraseHash.Length);
            Assert.NotEmpty(entity.DisplayId);
            Assert.Null(entity.LastName);
            Assert.False(entity.IsDeleted);
            Assert.Null(entity.ModifiedAt);

            ClassFixture.NewUser = entity;
        }

        [OrderedFact]
        public async Task Should_Throw_While_Adding_User_Duplicate_Name()
        {
            IEntityRepository<User> repo = CreateUserRepository();

            User user = new User
            {
                FirstName = "Al",
                DisplayId = ClassFixture.NewUser.DisplayId,
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
            IEntityRepository<User> repo = CreateUserRepository();
            User entity = await repo.GetByIdAsync(ClassFixture.NewUser.Id);

            Assert.Equal(ClassFixture.NewUser.Id, entity.Id);
            Assert.Equal(ClassFixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(ClassFixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(ClassFixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(ClassFixture.NewUser.LastName, entity.LastName);
            Assert.Equal(entity.JoinedAt, ClassFixture.NewUser.JoinedAt, new DateTimeEqualityComparer());
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Name()
        {
            string username = ClassFixture.NewUser.DisplayId.ToUpper();

            IUserRepository sut = CreateUserRepository();
            User entity = await sut.GetByNameAsync(username);

            Assert.Equal(ClassFixture.NewUser.Id, entity.Id);
            Assert.Equal(ClassFixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(ClassFixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(ClassFixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(ClassFixture.NewUser.LastName, entity.LastName);
            Assert.Equal(entity.JoinedAt, ClassFixture.NewUser.JoinedAt, new DateTimeEqualityComparer());
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [OrderedFact]
        public async Task Should_Throw_While_Getting_Non_Existing_User()
        {
            const string id = "non-existing-id";
            IEntityRepository<User> sut = CreateUserRepository();

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
                Id = ClassFixture.NewUser.Id,
                DisplayId = newDisplayId,
                FirstName = newFName,
                LastName = newLName,
                PassphraseHash = newPassphraseHash,
                JoinedAt = ClassFixture.NewUser.JoinedAt
            };
            DateTime timeBeforeTestAction = DateTime.UtcNow;

            IEntityRepository<User> sut = CreateUserRepository();
            User updatedEntity = await sut.UpdateAsync(user);

            Assert.Same(user, updatedEntity);
            Assert.Equal(ClassFixture.NewUser.Id, updatedEntity.Id);
            Assert.Equal(newFName, updatedEntity.FirstName);
            Assert.Equal(newLName, updatedEntity.LastName);
            Assert.Equal(newDisplayId, updatedEntity.DisplayId);
            Assert.Equal(newPassphraseHash, updatedEntity.PassphraseHash);
            Assert.NotNull(updatedEntity.ModifiedAt);
            Assert.True(updatedEntity.ModifiedAt > timeBeforeTestAction);
            Assert.Equal(updatedEntity.JoinedAt, ClassFixture.NewUser.JoinedAt, new DateTimeEqualityComparer());

            updatedEntity.CopyTo(ClassFixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Not_Override_Modified_Date()
        {
            DateTime modificationDate = DateTime.Today.AddDays(-1);

            User user = ClassFixture.NewUser;
            user.ModifiedAt = modificationDate;

            IEntityRepository<User> sut = CreateUserRepository();
            User updatedEntity = await sut.UpdateAsync(user);

            Assert.Equal(modificationDate.ToUniversalTime(), updatedEntity.ModifiedAt,
                new NullableDateTimeEqualityComparer());

            updatedEntity.CopyTo(ClassFixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Set_User_Token()
        {
            const string token = "api_token";

            IUserRepository sut = CreateUserRepository();

            await sut.SetTokenForUserAsync(ClassFixture.NewUser.Id, token);

            ClassFixture.NewUser.Token = token;
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Token()
        {
            IUserRepository sut = CreateUserRepository();

            User entity = await sut.GetByTokenAsync(ClassFixture.NewUser.Token);

            Assert.Equal(ClassFixture.NewUser.Token, entity.Token);

            Assert.Equal(ClassFixture.NewUser.Id, entity.Id);
            Assert.Equal(ClassFixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(ClassFixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(ClassFixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(ClassFixture.NewUser.LastName, entity.LastName);
            Assert.Equal(ClassFixture.NewUser.JoinedAt, entity.JoinedAt, new DateTimeEqualityComparer());
            Assert.NotNull(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);

            entity.CopyTo(ClassFixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Throw_Getting_User_By_Invalid_Token()
        {
            IUserRepository sut = CreateUserRepository();

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                sut.GetByTokenAsync("invalid_token")
            );

            Assert.Contains("token", exception.Message);
        }
    }
}