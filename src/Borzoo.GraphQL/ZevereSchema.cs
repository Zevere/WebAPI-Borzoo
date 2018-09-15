using GraphQL;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public class ZevereSchema : Schema
    {
        public ZevereSchema(FuncDependencyResolver resolver)
        {
            DependencyResolver = resolver;
            Query = resolver.Resolve<ZevereQuery>();
            Mutation = resolver.Resolve<ZevereMutation>();
        }
    }
}