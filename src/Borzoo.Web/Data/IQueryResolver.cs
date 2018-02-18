using System.Threading.Tasks;
using Borzoo.Web.Models.User;
using GraphQL.Types;

namespace Borzoo.Web.Data
{
    public interface IQueryResolver
    {
        Task<User> ResolveUserAsync(ResolveFieldContext<object> context);
    }
}