using Borzoo.GraphQL.Types;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public class ZevereMutation : ObjectGraphType
    {
        public ZevereMutation(IQueryResolver resolver)
        {
            Name = nameof(ZevereMutation);

            Field<UserType>(
                "createUser",
                "Create a new user account",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<UserInputType>> {Name = "user"}
                ),
                resolver.CreateUserAsync
            );

            Field<TaskListType>(
                "createList",
                "Create a new task list. Current user will be its owner.",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "owner",
                        Description = "Username of owner",
                    },
                    new QueryArgument<NonNullGraphType<TaskListInputType>>
                    {
                        Name = "list",
                        Description = "Parameters for creating a new task list",
                    }
                ),
                resolver.CreateTaskListAsync
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