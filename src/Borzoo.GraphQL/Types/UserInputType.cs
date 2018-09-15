using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class UserInputType : InputObjectGraphType<UserCreationDto>
    {
        public UserInputType()
        {
            Name = "UserInput";
            Description = "Input for creating a new user or organization";

            Field(_ => _.Name)
                .Description("The desired user name. User names are case insensitive. " +
                             "Valid characters are ASCII alphanumeric characters, _, ., and -.");
            Field(_ => _.FirstName).Description("User's first name");
            Field(_ => _.Passphrase).Description("Passphrase as clear text");

            Field(_ => _.LastName, true).Description("User's last name");
            Field(_ => _.Members, true).Description("If organization user, user ID of team members");
        }
    }
}