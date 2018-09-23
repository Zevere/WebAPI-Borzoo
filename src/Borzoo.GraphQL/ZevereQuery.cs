using Borzoo.GraphQL.Types;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public class ZevereQuery : ObjectGraphType
    {
        public ZevereQuery(IQueryResolver resolver)
        {
            Name = nameof(ZevereQuery);

            Field<UserType>("user",
                "Get user account information by id",
                new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "userId"}),
                resolver.GetUserAsync
            );
        }
    }
}