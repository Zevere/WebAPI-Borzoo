using Borzoo.Web.Data;
using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public class ZevereQuery : ObjectGraphType
    {
        public ZevereQuery(IQueryResolver resolver)
        {
            Name = nameof(ZevereQuery);

            Field<UserType>("user",
                "User",
                new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}),
                resolver.GetUserAsync
            );
        }
    }
}