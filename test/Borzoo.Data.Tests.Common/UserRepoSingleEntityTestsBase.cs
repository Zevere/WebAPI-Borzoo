using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Tests.Common.Framework;
using Xunit;

namespace Borzoo.Data.Tests.Common
{
    public abstract class UserRepoSingleEntityTestsBase
    {
        protected readonly IUserRepoSingleEntityFixture Fixture;

        protected Func<IUserRepository> CreateUserRepository { get; }

        protected UserRepoSingleEntityTestsBase(IUserRepoSingleEntityFixture fixture,
            Func<IUserRepository> repoResolver)
        {
            Fixture = fixture;
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

            Fixture.NewUser = entity;
        }

        [OrderedFact]
        public async Task Should_Throw_While_Adding_User_Duplicate_Name()
        {
            IEntityRepository<User> repo = CreateUserRepository();

            User user = new User
            {
                FirstName = "Al",
                DisplayId = Fixture.NewUser.DisplayId,
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
            IEntityRepository<User> repo = CreateUserRepository();
            User entity = await repo.GetByIdAsync(Fixture.NewUser.Id);

            Assert.Equal(Fixture.NewUser.Id, entity.Id);
            Assert.Equal(Fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(Fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(Fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(Fixture.NewUser.LastName, entity.LastName);
            Assert.InRange(entity.JoinedAt.Ticks,
                Fixture.NewUser.JoinedAt.Ticks - 100_000,
                Fixture.NewUser.JoinedAt.Ticks + 100_000);
            Assert.Null(entity.ModifiedAt);
            Assert.False(entity.IsDeleted);
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Name()
        {
            string username = Fixture.NewUser.DisplayId.ToUpper();

            IUserRepository sut = CreateUserRepository();
            User entity = await sut.GetByNameAsync(username);

            Assert.Equal(Fixture.NewUser.Id, entity.Id);
            Assert.Equal(Fixture.NewUser.DisplayId, entity.DisplayId);
            Assert.Equal(Fixture.NewUser.PassphraseHash, entity.PassphraseHash);
            Assert.Equal(Fixture.NewUser.FirstName, entity.FirstName);
            Assert.Equal(Fixture.NewUser.LastName, entity.LastName);
            Assert.InRange(entity.JoinedAt.Ticks,
                Fixture.NewUser.JoinedAt.Ticks - 100_000,
                Fixture.NewUser.JoinedAt.Ticks + 100_000);
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
                Id = Fixture.NewUser.Id,
                DisplayId = newDisplayId,
                FirstName = newFName,
                LastName = newLName,
                PassphraseHash = newPassphraseHash,
                JoinedAt = Fixture.NewUser.JoinedAt
            };
            DateTime timeBeforeTestAction = DateTime.UtcNow;

            IEntityRepository<User> sut = CreateUserRepository();
            User updatedEntity = await sut.UpdateAsync(user);

            Assert.Same(user, updatedEntity);
            Assert.Equal(Fixture.NewUser.Id, updatedEntity.Id);
            Assert.Equal(newFName, updatedEntity.FirstName);
            Assert.Equal(newLName, updatedEntity.LastName);
            Assert.Equal(newDisplayId, updatedEntity.DisplayId);
            Assert.Equal(newPassphraseHash, updatedEntity.PassphraseHash);
            Assert.NotNull(updatedEntity.ModifiedAt);
            Assert.True(updatedEntity.ModifiedAt > timeBeforeTestAction);
            Assert.InRange(updatedEntity.JoinedAt.Ticks,
                Fixture.NewUser.JoinedAt.Ticks - 100_000,
                Fixture.NewUser.JoinedAt.Ticks + 100_000);

            updatedEntity.CopyTo(Fixture.NewUser);
        }

        [OrderedFact]
        public async Task Should_Not_Override_Modified_Date()
        {
            DateTime modificationDate = DateTime.Today.AddDays(-1);

            User user = Fixture.NewUser;
            user.ModifiedAt = modificationDate;

            IEntityRepository<User> sut = CreateUserRepository();
            User updatedEntity = await sut.UpdateAsync(user);

            Assert.Equal(modificationDate.ToUniversalTime(), updatedEntity.ModifiedAt);
//            Assert.InRange(modificationDate.ToUniversalTime().Ticks,
//                updatedEntity.ModifiedAt.Value.Ticks - 100_000,
//                updatedEntity.ModifiedAt.Value.Ticks + 100_000
//            );
        }
    }
}