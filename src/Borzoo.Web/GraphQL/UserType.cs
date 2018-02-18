﻿using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public class UserType : ObjectGraphType<Models.User.User>
    {
        public UserType()
        {
            Name = "User";
            Description = "A Zevere user";
            
            Field(_ => _.Id)
                .Description("User name");
            
            Field(_ => _.FirstName)
                .Description("First name");
            
            Field(_ => _.LastName)
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