using Borzoo.GraphQL;
using Borzoo.GraphQL.Types;
using Borzoo.Web.GraphQL;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.Helpers
{
    public static class GraphQLExtensions
    {
        public static IServiceCollection AddGraphQL(this IServiceCollection services)
        {
            services.AddTransient<IQueryResolver, QueryResolver>();
            
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<UserType>();
            services.AddSingleton<UserInput>();

            services.AddSingleton<ZevereQuery>();
            services.AddSingleton<ZevereMutation>();
            services.AddSingleton<ISchema, ZevereSchema>(_ => new ZevereSchema(
                new FuncDependencyResolver(_.GetRequiredService)
            ));
            
            return services;
        }
    }
}