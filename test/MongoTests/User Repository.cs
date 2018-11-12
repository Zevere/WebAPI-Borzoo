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
            // 1. insert a new user into the collection
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

            // 2. ensure entity is updated
            {
                Assert.Equal("Charles", user.FirstName);
                Assert.Equal("chuck", user.DisplayId);
                Assert.Equal("some secret-PASS", user.PassphraseHash);
                Assert.NotNull(user.Id);
                Assert.True(ObjectId.TryParse(user.Id, out _), "User ID should be a Mongo ObjectID.");
                Assert.InRange(user.JoinedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);

                Assert.Null(user.LastName);
                Assert.Null(user.Token);
                Assert.False(user.IsDeleted);
                Assert.Null(user.ModifiedAt);
            }

            // 3. ensure entity is created properly
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

//        [OrderedFact(DisplayName = "Should set token for the user")]
//        public async Task Should_Set_User_Token()
//        {
//            string userId = _fixture.UserId;
//
//            IUserRepository repo = new UserRepository(
//                _fixture.Database.GetCollection<User>("users")
//            );
//
//            await repo.SetTokenForUserAsync(userId, "~~Token~~");
//        }
//
//        [OrderedFact(DisplayName = "Should get the user by his username")]
//        public async Task Should_User_Get_By_Name()
//        {
//            string userId = _fixture.UserId;
//
//            IUserRepository repo = new UserRepository(
//                _fixture.Database.GetCollection<User>("users")
//            );
//
//            User user = await repo.GetByNameAsync("chuck");
//
//            Assert.Same(user, user);
//            Assert.Equal(userId, user.Id);
//            Assert.Equal("Charles", user.FirstName);
//            Assert.Equal("chuck", user.DisplayId);
//            Assert.Equal("secret_passphrase", user.PassphraseHash);
//            Assert.Equal("~~Token~~", user.Token);
//
//            Assert.Null(user.LastName);
//            Assert.False(user.IsDeleted);
//            Assert.Null(user.ModifiedAt);
//        }
    }
}
