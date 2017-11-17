using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Web.Data
{
    public class DataSeeder
    {
        private readonly IUserRepository _useRepo;

        public DataSeeder(IUserRepository useRepo)
        {
            _useRepo = useRepo;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await IsAlreadySeededAsync())
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
                await _useRepo.AddAsync(user, cancellationToken);
            }
        }

        private async Task<bool> IsAlreadySeededAsync()
        {
            bool userExists;
            try
            {
                await _useRepo.GetByNameAsync("alICE0");
                userExists = true;
            }
            catch (EntityNotFoundException)
            {
                userExists = false;
            }
            return userExists;
        }
    }
}