using GraphQL;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    /// <summary>
    /// Represents the Zevere GraphQL schema
    /// </summary>
    public class ZevereSchema : Schema
    {
        /// <inheritdoc />
        public ZevereSchema(FuncDependencyResolver resolver)
        {
            DependencyResolver = resolver;
            Query = resolver.Resolve<ZevereQuery>();
            Mutation = resolver.Resolve<ZevereMutation>();
        }
    }
}
