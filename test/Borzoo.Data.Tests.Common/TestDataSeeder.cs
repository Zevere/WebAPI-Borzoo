using System.Linq;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using GenFu;
using GenFu.ValueGenerators.Lorem;
using GenFu.ValueGenerators.People;

namespace Borzoo.Data.Tests.Common
{
    public static class TestDataSeeder
    {
        static TestDataSeeder()
        {
            GenFu.GenFu.Configure<User>()
                .Fill(_ => _.DisplayId, Names.UserName)
                .Fill(_ => _.FirstName, Names.FirstName)
                .Fill(_ => _.LastName).WithRandom(new[] {null, Names.LastName()})
                .Fill(_ => _.PassphraseHash, Lorem.GenerateWords(3));

            GenFu.GenFu.Configure<TaskList>()
                .Fill(_ => _.DisplayId, Names.UserName)
                .Fill(_ => _.Title, Names.Title);
        }

        public static async Task<User[]> SeedUsersAsync(IUserRepository userRepo)
        {
            var testUsers = GenerateTestUsers();

            foreach (var user in testUsers)
            {
                try
                {
                    await userRepo.AddAsync(user);
                }
                catch (DuplicateKeyException)
                {
                }
            }

            return testUsers;
        }

        public static async Task<TaskList[]> SeedTaskListAsync(ITaskListRepository tasklistRepo)
        {
            var tasklists = GenerateTestTaskLists();

            foreach (var tl in tasklists)
            {
                try
                {
                    await tasklistRepo.AddAsync(tl);
                }
                catch (DuplicateKeyException)
                {
                }
            }

            return tasklists;
        }

        private static User[] GenerateTestUsers()
        {
            User[] testUsers =
            {
                new User
                {
                    DisplayId = "bobby",
                    FirstName = "Bob",
                    LastName = "Boo",
                    PassphraseHash = "secret_passphrase2"
                },
                new User
                {
                    DisplayId = "alice0",
                    FirstName = "Alice",
                    PassphraseHash = "secret_passphrase"
                },
            };

            testUsers = testUsers
                .Concat(A.ListOf<User>(5))
                .ToArray();

            foreach (var u in testUsers)
            {
                u.Id = null;
                u.Token = null;
                u.ModifiedAt = null;
                u.IsDeleted = false;
            }

            return testUsers;
        }

        private static TaskList[] GenerateTestTaskLists()
        {
            TaskList[] testLists =
            {
                new TaskList
                {
                    DisplayId = "list",
                    Title = "List",
                },
            };

            testLists = testLists
                .Concat(A.ListOf<TaskList>(5))
                .ToArray();

            foreach (var l in testLists)
            {
                l.Id = null;
                l.OwnerId = null;
                l.ModifiedAt = null;
                l.IsDeleted = false;
            }

            return testLists;
        }
    }
}