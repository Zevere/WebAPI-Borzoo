using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.GraphQL
{
    public class ZevereSchema : Schema
    {
        private readonly IServiceProvider _serviceProvider;

        public ZevereSchema(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            base.ResolveType = ResolveType;
            Query = _serviceProvider.GetRequiredService<UserQuery>();
        }

        private IGraphType ResolveType(Type type) =>
            _serviceProvider.GetRequiredService(type) as IGraphType;
    }
}