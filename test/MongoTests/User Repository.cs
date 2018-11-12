using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Framework;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoTests.Shared;
using Xunit;

namespace MongoTests
{
    [Collection("user repository")]
    public class UserRepoTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fxt;

        public UserRepoTests(DatabaseFixture fixture)
        {
            _fxt = fixture;
        }

        [OrderedFact("Should create a new user")]
        public async Task Should_Add_New_User()
        {
            // insert a new user into the collection
            User user;
            {
                IUserRepository userRepo = new UserRepository(
                    _fxt.Database.GetCollection<User>("users")
                );
                user = new User
                {
                    FirstName = "Charles",
                    DisplayId = "chuck",
                    PassphraseHash = "some secret-PASS"
                };
                await userRepo.AddAsync(user);
            }

            // ensure user object is updated
            {
                Assert.Equal("Charles", user.FirstName);
                Assert.Equal("chuck", user.DisplayId);
                Assert.Equal("some secret-PASS", user.PassphraseHash);
                Assert.NotNull(user.Id);
                Assert.True(ObjectId.TryParse(user.Id, out _), "User ID should be a Mongo ObjectID.");
                Assert.InRange(user.JoinedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);

                Assert.Null(user.LastName);
                Assert.Null(user.Token);
                Assert.Null(user.ModifiedAt);
            }

            // ensure user document is created in the collection
            {
                BsonDocument userDocument = _fxt.Database
                    .GetCollection<BsonDocument>("users")
                    .FindSync(FilterDefinition<BsonDocument>.Empty)
                    .Single();

                Assert.Equal(
                    BsonDocument.Parse($@"{{
                        _id: ObjectId(""{user.Id}""),
                        name: ""chuck"",
                        pass: ""some secret-PASS"",
                        fname: ""Charles"",
                        joined: ISODate(""{user.JoinedAt:O}"")
                    }}"),
                    userDocument
                );
            }
        }

        [OrderedFact("Should get user 'chuck' by his username")]
        public async Task Should_Get_User_By_Name()
        {
            IUserRepository userRepo = new UserRepository(
                _fxt.Database.GetCollection<User>("users")
            );

            // get user by his username
            User user = await userRepo.GetByNameAsync("CHuck");

            Assert.Equal("Charles", user.FirstName);
            Assert.Equal("chuck", user.DisplayId);
            Assert.Equal("some secret-PASS", user.PassphraseHash);
            Assert.NotNull(user.Id);
            Assert.True(ObjectId.TryParse(user.Id, out _), "User ID should be a Mongo ObjectID.");
            Assert.InRange(user.JoinedAt, DateTime.UtcNow.AddSeconds(-30), DateTime.UtcNow);

            Assert.Null(user.LastName);
            Assert.Null(user.Token);
            Assert.Null(user.ModifiedAt);
        }

        [OrderedFact("Should get a user by its Object ID")]
        public async Task Should_Get_User_By_ObjectId()
        {
            IUserRepository userRepo = new UserRepository(
                _fxt.Database.GetCollection<User>("users")
            );

            // insert a new user into the collection
            string userId;
            {
                User newUser = new User
                {
                    FirstName = "Dave",
                    DisplayId = "dav3",
                    PassphraseHash = "passphrase~hash"
                };
                await userRepo.AddAsync(newUser);

                userId = newUser.Id;
            }

            User user = await userRepo.GetByIdAsync(userId);
            Assert.Equal("Dave", user.FirstName);
            Assert.Equal("dav3", user.DisplayId);
            Assert.Equal("passphrase~hash", user.PassphraseHash);
            Assert.True(ObjectId.TryParse(user.Id, out _), "User ID should be a Mongo ObjectID.");
            Assert.InRange(user.JoinedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);

            Assert.Null(user.LastName);
            Assert.Null(user.Token);
            Assert.Null(user.ModifiedAt);
        }

        [OrderedFact("Should throw when adding another user with 'chuck' username")]
        public async Task Should_Throw_When_Add_Duplicate_Username()
        {
            IUserRepository userRepo = new UserRepository(
                _fxt.Database.GetCollection<User>("users")
            );

            await Assert.ThrowsAsync<DuplicateKeyException>(() =>
                userRepo.AddAsync(new User { DisplayId = "CHuck" })
            );
        }

        [OrderedFact("Should throw when getting non-existing user by username")]
        public async Task Should_Throw_When_Get_NonExisting_Username()
        {
            IUserRepository userRepo = new UserRepository(
                _fxt.Database.GetCollection<User>("users")
            );

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                userRepo.GetByNameAsync(@"username ¯\_(ツ)_/¯")
            );
        }

        [OrderedFact("Should throw when getting non-existing user by invalid object id")]
        public async Task Should_Throw_When_Get_NonExisting_Invalid_UserId()
        {
            IUserRepository userRepo = new UserRepository(
                _fxt.Database.GetCollection<User>("users")
            );

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                userRepo.GetByIdAsync(@"object id ಠ_ಠ")
            );
        }

        [OrderedFact("Should throw when getting non-existing user by object id")]
        public async Task Should_Throw_When_Get_NonExisting_UserId()
        {
            IUserRepository userRepo = new UserRepository(
                _fxt.Database.GetCollection<User>("users")
            );

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                userRepo.GetByIdAsync("5be91279beb91c512357c902")
            );
        }
    }
}
