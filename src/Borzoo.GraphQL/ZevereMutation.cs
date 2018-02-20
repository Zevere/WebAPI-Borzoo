using Borzoo.GraphQL.Types;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public class ZevereMutation : ObjectGraphType
    {
        public ZevereMutation(IQueryResolver resolver)
        {
            Name = nameof(ZevereMutation);

            Field<UserType>("createUser",
                "Create a new user", new QueryArguments(
                    new QueryArgument<NonNullGraphType<UserInput>> {Name = "user"}
                ),
                resolver.CreateUserAsync
            );
        }
    }
}