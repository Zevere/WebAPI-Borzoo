using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Borzoo.Web.Middlewares.GraphQL
{
    public class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly GraphQLSettings _settings;

        private readonly IDocumentExecuter _executer;

        private readonly IDocumentWriter _writer;

        public GraphQLMiddleware(
            RequestDelegate next,
            IDocumentExecuter executer,
            IDocumentWriter writer,
            GraphQLSettings settings = default)
        {
            _next = next;
            _executer = executer;
            _writer = writer;
            _settings = settings ?? new GraphQLSettings();
        }

        public Task Invoke(HttpContext context, ISchema schema) =>
            IsGraphQLRequest(context)
                ? ExecuteAsync(context, schema)
                : _next(context);

        private bool IsGraphQLRequest(HttpContext context) =>
            context.Request.Path.StartsWithSegments(_settings.Path) &&
            string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase);

        private async Task ExecuteAsync(HttpContext context, ISchema schema)
        {
            string body;
            using (var streamReader = new StreamReader(context.Request.Body))
                body = await streamReader.ReadToEndAsync()
                    .ConfigureAwait(false);

            GraphQLRequest gqlReq;
            try
            {
                gqlReq = JsonConvert.DeserializeObject<GraphQLRequest>(body);
            }
            catch (JsonException)
            {
                // ToDo Log
                context.Response.StatusCode = 400;
                return;
            }

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = gqlReq.Query;
                _.OperationName = gqlReq.OperationName;
                _.Inputs = gqlReq.Variables.ToInputs();
                _.UserContext = _settings.BuildUserContext?.Invoke(context);
            });

            await WriteResponseAsync(context, result)
                .ConfigureAwait(false);
        }

        private async Task WriteResponseAsync(HttpContext context, ExecutionResult result)
        {
            string json = _writer.Write(result);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json)
                .ConfigureAwait(false);
        }
    }
}