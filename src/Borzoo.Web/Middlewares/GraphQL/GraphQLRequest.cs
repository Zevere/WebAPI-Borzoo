using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Middlewares.GraphQL
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GraphQLRequest
    {
        public string OperationName { get; set; }
        
        public string Query { get; set; }

        public JObject Variables { get; set; }
    }
}