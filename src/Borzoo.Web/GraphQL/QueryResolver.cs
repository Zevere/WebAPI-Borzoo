using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Web.Models;
using Borzoo.Web.Models.User;
using GraphQL.Types;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.GraphQL
{
    public class QueryResolver : IQueryResolver
    {
        private readonly IUserRepository _userRepo;

        public QueryResolver(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserDto> GetUserAsync(ResolveFieldContext<object> context)
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

            return (UserDto) entity;
        }

        public async Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context)
        {
            var dto = context.GetArgument<UserCreationDto>("user");

            var entity = (UserEntity) dto;
            try
            {
                await _userRepo.AddAsync(entity, context.CancellationToken);
            }
            catch (DuplicateKeyException)
            {
                var err = new Error("duplicate key")
                {
                    Path = new[] {"user"}
                };
                context.Errors.Add(err);
                return default;
            }

            return (UserDto) entity;
        }
    }
}