using Borzoo.Web.Data;
using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public class UserQuery : ObjectGraphType
    {
        public UserQuery(IQueryResolver resolver)
        {
            Name = nameof(UserQuery);

            Field<UserType>("user",
                "User",
                new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}),
                resolver.ResolveUserAsync
            );
        }
    }
}