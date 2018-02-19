using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public class UserInput : InputObjectGraphType
    {
        public UserInput()
        {
            Name = "UserInput";
            Description = "Input for creating a new user or organization";

            Field<NonNullGraphType<StringGraphType>>("name", "The desired user name");
            Field<NonNullGraphType<StringGraphType>>("passphrase", "Passphrase as clear text");
            Field<NonNullGraphType<StringGraphType>>("firstName", "User's first name");
            Field<StringGraphType>("lastName", "User's last name");
            Field<ListGraphType<StringGraphType>>("members", "If organization user, user ID of team members");
        }
    }
}