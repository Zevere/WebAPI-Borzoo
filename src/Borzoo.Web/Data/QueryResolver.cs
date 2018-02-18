using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Web.GraphQL;
using Borzoo.Web.Models.User;
using GraphQL.Types;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Data
{
    public class QueryResolver : IQueryResolver
    {
        private readonly IUserRepository _userRepo;

        public QueryResolver(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> ResolveUserAsync(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("id");
            UserEntity entity;
            try
            {
                entity = await _userRepo.GetByNameAsync(username, cancellationToken: context.CancellationToken);
            }
            catch (EntityNotFoundException)
            {
                var err = new Error("not found")
                {
                    Path = new[] {"user"}
                };
                context.Errors.Add(err);
                return default;
            }
            
            return (User) entity;
        }
    }
}