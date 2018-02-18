using Borzoo.Web.Data;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.GraphQL
{
    public static class Extensions
    {
        public static IServiceCollection AddGraphQL(this IServiceCollection services)
        {
            services.AddTransient<IQueryResolver, QueryResolver>();
            
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<UserType>();

            services.AddSingleton<UserQuery>();
            services.AddSingleton<ISchema, ZevereSchema>();
            
            return services;
        }
    }
}