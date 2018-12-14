using System.Security.Principal;

namespace Borzoo.Web.Middlewares.BasicAuth
{
    /// <summary>
    /// User identity from HTTP basic authentication
    /// </summary>
    public class BasicAuthIdentity : IIdentity
    {
        /// <inheritdoc />
        public string AuthenticationType => "Basic";

        /// <inheritdoc />
        public bool IsAuthenticated { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public BasicAuthIdentity(string name, bool isAuthenticated = true)
        {
            Name = name;
            IsAuthenticated = isAuthenticated;
        }
    }
}
