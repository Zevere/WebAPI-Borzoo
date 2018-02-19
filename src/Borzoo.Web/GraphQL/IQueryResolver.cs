using System.Threading.Tasks;
using Borzoo.Web.Models;
using Borzoo.Web.Models.User;
using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public interface IQueryResolver
    {
        Task<UserDto> GetUserAsync(ResolveFieldContext<object> context);
        
        Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context);
    }
}