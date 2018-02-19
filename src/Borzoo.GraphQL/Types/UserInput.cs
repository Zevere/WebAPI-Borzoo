using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public class UserInput : InputObjectGraphType<UserCreationDto>
    {
        public UserInput()
        {
            Name = "UserInput";
            Description = "Input for creating a new user or organization";

            Field(_ => _.Name).Description("The desired user name");
            Field(_ => _.FirstName).Description("User's first name");
            Field(_ => _.Passphrase).Description("Passphrase as clear text");

            Field(_ => _.LastName, true).Description("User's last name");
            Field(_ => _.Members, true).Description("If organization user, user ID of team members");
        }
    }
}