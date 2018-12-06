using Borzoo.GraphQL.Types;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    /// <summary>
    /// Represents root of the graph for mutation operations
    /// </summary>
    public class ZevereMutation : ObjectGraphType
    {
        /// <inheritdoc />
        public ZevereMutation(IQueryResolver resolver)
        {
            Name = nameof(ZevereMutation);
            Description = "Root of the graph for mutation operations";

            Field<UserType>(
                "createUser",
                "Create a new user account",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<UserInputType>> { Name = "user" }
                ),
                resolver.CreateUserAsync
            );

            Field<UserType>(
                "login",
                "Login to a user account",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<LoginInputType>> { Name = "login" }
                ),
                resolver.LoginAsync
            );

            Field<TaskListType>(
                "createList",
                "Create a new task list.",
                // ToDo use auth tokens and current logged in user will be the
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "owner",
                        Description = "Username of the list owner",
                    },
                    new QueryArgument<NonNullGraphType<TaskListInputType>>
                    {
                        Name = "list",
                        Description = "Parameters for creating a new task list",
                    }
                ),
                resolver.CreateTaskListAsync
            );

            Field<NonNullGraphType<BooleanGraphType>>(
                "deleteList",
                "Delete a task list owned by the user",
                // ToDo use auth tokens and current logged in user will be the
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "owner",
                        Description = "Username of the list owner",
                    },
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "list",
                        Description = "Name of the list to be deleted",
                    }
                ),
                resolver.DeleteTaskListAsync
            );

            Field<TaskItemType>(
                "addTask",
                "Add a new task to the list",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "owner",
                        Description = "Username of owner"
                    },
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "list",
                        Description = "ID of the task list"
                    },
                    new QueryArgument<NonNullGraphType<TaskItemInputType>>
                    {
                        Name = "task",
                        Description = "Parameters for creating a new task item",
                    }
                ),
                resolver.CreateTaskItemAsync
            );
        }
    }
}
