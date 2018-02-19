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
                .Description("User name");
            
            Field(_ => _.FirstName)
                .Description("First name");
            
            Field(_ => _.LastName, true)
                .Description("Last name");
            
            Field(_ => _.DisplayName)
                .Description("Display name");
            
            Field(_ => _.DaysJoined)
                .Description("Days joined");
            
            Field(_ => _.JoinedAt)
                .Description("Join date");
        }
    }
}