using Borzoo.Web.Data;
using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public class ZevereMutation : ObjectGraphType
    {
        public ZevereMutation(IQueryResolver resolver)
        {
            Name = nameof(ZevereMutation);

            Field<UserType>("CreateUser", "Create a new user", new QueryArguments(
                    new QueryArgument<NonNullGraphType<UserInput>> {Name = "user"}
                ),
                resolver.ResolveUserAsync
            );
        }
    }
}