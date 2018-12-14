using Borzoo.GraphQL;
using Borzoo.GraphQL.Types;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.Extensions
{
    internal static class GraphQlExtensions
    {
        /// <summary>
        /// Adds GraphQL services to the app's service collection
        /// </summary>
        public static void AddGraphQl(this IServiceCollection services)
        {
            services.AddTransient<IQueryResolver, QueryResolver>();

            services.AddSingleton<IDocumentWriter, DocumentWriter>();

            services.AddSingleton<UserType>();
            services.AddSingleton<UserInputType>();
            services.AddSingleton<LoginInputType>();
            services.AddSingleton<TaskListType>();
            services.AddSingleton<TaskListInputType>();
            services.AddSingleton<TaskItemType>();
            services.AddSingleton<TaskItemInputType>();

            // ToDo use singletons
            services.AddScoped<ZevereQuery>();
            services.AddScoped<ZevereMutation>();
            services.AddScoped<ISchema, ZevereSchema>(_ => new ZevereSchema(
                new FuncDependencyResolver(_.GetRequiredService)
            ));

            services.AddGraphQl(schema =>
            {
                schema.SetQueryType<ZevereQuery>();
                schema.SetMutationType<ZevereMutation>();
            });
        }
    }
}
