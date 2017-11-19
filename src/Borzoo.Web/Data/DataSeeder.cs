using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite;

namespace Borzoo.Web.Data
{
    public class DataSeeder
    {
        private readonly IUserRepository _useRepo;

        public DataSeeder(IUserRepository useRepo)
        {
            _useRepo = useRepo;
        }

        public void SeedData(string migrationsSqlFile)
        {
            ApplyMigrations(migrationsSqlFile);

            if (IsAlreadySeeded())
            {
                return;
            }

            User[] testUsers =
            {
                new User
                {
                    DisplayId = "alice0",
                    FirstName = "Alice",
                    PassphraseHash = "secret_passphrase"
                },
                new User
                {
                    DisplayId = "bobby",
                    FirstName = "Bob",
                    LastName = "Boo",
                    PassphraseHash = "secret_passphrase2"
                },
            };

            foreach (var user in testUsers)
            {
                _useRepo.AddAsync(user).GetAwaiter().GetResult();
            }
        }

        private bool IsAlreadySeeded()
        {
            bool userExists;
            try
            {
                _useRepo.GetByNameAsync("alICE0").GetAwaiter().GetResult();
                userExists = true;
            }
            catch (EntityNotFoundException)
            {
                userExists = false;
            }
            return userExists;
        }

        private void ApplyMigrations(string migrationsSqlFile)
        {
            DatabaseInitializer.EnsureMigrationsApplied(migrationsSqlFile);
        }
    }
}