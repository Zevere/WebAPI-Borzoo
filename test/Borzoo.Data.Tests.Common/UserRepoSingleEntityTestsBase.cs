using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
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
            IUserRepository repo = CreateUserRepository();

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
            IUserRepository repo = CreateUserRepository();

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
            IUserRepository repo = CreateUserRepository();
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
    }
}