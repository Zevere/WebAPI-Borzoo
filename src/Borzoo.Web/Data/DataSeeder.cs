using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data
{
    public class DataSeeder
    {
        private readonly IEntityRepository<User> _useRepository;

        public DataSeeder(IEntityRepository<User> useRepository)
        {
            _useRepository = useRepository;
        }

        public async Task Seed(CancellationToken cancellationToken = default)
        {
            User[] testUsers =
            {
                new User
                {
                    DisplayId = "alice0",
                    FirstName = "Alice"
                },
                new User
                {
                    DisplayId = "bobby",
                    FirstName = "Bob",
                    LastName = "Boo"
                },
            };

            foreach (var user in testUsers)
            {
                await _useRepository.AddAsync(user, cancellationToken);
            }
        }
    }
}