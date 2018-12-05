using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class UserType : ObjectGraphType<UserDto>
    {
        public UserType(IQueryResolver queryResolver)
        {
            Name = "User";
            Description = "A Zevere user account";

            Field(_ => _.Id)
                .Description("User name");

            Field(_ => _.FirstName)
                .Description("First name");

            Field(_ => _.LastName, true)
                .Description("Last name");

            Field(_ => _.Token, true)
                .Description("Authentication token");

            Field(_ => _.DaysJoined)
                .Description("Number of days this user has joined");

            Field(_ => _.JoinedAt)
                .Description("The date account was created in UTC format. Time should be set to the midnight.");

            Field<TaskListType>(
                "list",
                "A list that this user has access to",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "listId",
                        Description = "Name of the list",
                    }
                ),
                queryResolver.GetTaskListForUserAsync
            );

            Field<ListGraphType<TaskListType>>(
                "lists",
                "Tasks lists that this user has access to",
                resolve: queryResolver.GetAllTaskListsForUserAsync
            );
        }
    }
}
