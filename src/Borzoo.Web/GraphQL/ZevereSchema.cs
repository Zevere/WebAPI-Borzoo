using System;
using GraphQL;
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
            DependencyResolver = new FuncDependencyResolver(ResolveType);
            Query = _serviceProvider.GetRequiredService<ZevereQuery>();
            Mutation = _serviceProvider.GetRequiredService<ZevereMutation>();
        }

        private object ResolveType(Type type) =>
            _serviceProvider.GetRequiredService(type);
    }
}