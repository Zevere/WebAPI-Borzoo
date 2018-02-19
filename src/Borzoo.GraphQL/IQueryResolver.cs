using System.Threading.Tasks;
using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public interface IQueryResolver
    {
        Task<UserDto> GetUserAsync(ResolveFieldContext<object> context);
        
        Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context);
    }
}