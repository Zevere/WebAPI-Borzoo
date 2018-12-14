using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    /// <summary>
    /// Represents the user login input type
    /// </summary>
    public class LoginInputType : InputObjectGraphType<UserLoginDto>
    {
        /// <inheritdoc />
        public LoginInputType()
        {
            Name = "LoginInput";
            Description = "Input for user login";

            Field(_ => _.Username).Description("User ID");
            Field(_ => _.Passphrase).Description("Passphrase in clear text");
        }
    }
}
