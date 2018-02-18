using System;
using Microsoft.AspNetCore.Http;

namespace Borzoo.Web.Middlewares.GraphQL
{
    public class GraphQLSettings
    {
        public PathString Path { get; set; } = "/api/graphql";
        
        public Func<HttpContext, object> BuildUserContext { get; set; }
    }
}