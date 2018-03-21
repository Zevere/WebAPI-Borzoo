using System.IO;
using Borzoo.GraphQL;
using Borzoo.GraphQL.Types;
using Borzoo.Web.GraphQL;
using Borzoo.Web.Middlewares.GraphQL;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
            services.AddSingleton<TaskListType>();
            services.AddSingleton<TaskListInput>();
            services.AddSingleton<TaskItemType>();
            services.AddSingleton<TaskItemInput>();

            services.AddSingleton<ZevereQuery>();
            services.AddSingleton<ZevereMutation>();
            services.AddSingleton<ISchema, ZevereSchema>(_ => new ZevereSchema(
                new FuncDependencyResolver(_.GetRequiredService)
            ));

            return services;
        }

        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder app, GraphQLSettings settings = default)
        {
            app.UseMiddleware<GraphQLMiddleware>(settings);
            return app;
        }

        public static IApplicationBuilder UseGraphiQL(this IApplicationBuilder app, string requestPath,
            string filesBasePath)
        {
            var opts = new SharedOptions
            {
                RequestPath = requestPath,
                FileProvider = new PhysicalFileProvider(Path.GetFullPath(filesBasePath))
            };
            app.UseDefaultFiles(new DefaultFilesOptions(opts));
            app.UseStaticFiles(new StaticFileOptions(opts));

            return app;
        }
    }
}