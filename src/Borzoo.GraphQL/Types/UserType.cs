using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class UserType : ObjectGraphType<UserDto>
    {
        public UserType()
        {
            Name = "User";
            Description = "A Zevere user";
            
            Field(_ => _.Id)
                .Description("User's ID");
            
            Field(_ => _.FirstName)
                .Description("User's first name");
            
            Field(_ => _.LastName, true)
                .Description("User's last name");
            
            Field(_ => _.Token)
                .Description("Authentication token");
            
            Field(_ => _.DaysJoined)
                .Description("Number of days this user has joined.");
            
            Field(_ => _.JoinedAt)
                .Description("The date account was created in UTC format. Time should be set to the midnight.");
        }
    }
}