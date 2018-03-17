using System.Linq;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using GenFu;

namespace Borzoo.Data.Tests.Common
{
    public static class TestDataSeeder
    {
        public static async Task<User[]> SeedUsersAsync(IUserRepository userRepo)
        {
            var testUsers = GenerateTestUsers();

            foreach (var user in testUsers)
                await userRepo.AddAsync(user);

            return testUsers;
        }

        public static async Task<TaskList[]> SeedTaskListAsync(ITaskListRepository tasklistRepo)
        {
            var tasklists = GenerateTestTaskLists();

            foreach (var tl in tasklists)
                await tasklistRepo.AddAsync(tl);

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

            return testLists;
        }
    }
}