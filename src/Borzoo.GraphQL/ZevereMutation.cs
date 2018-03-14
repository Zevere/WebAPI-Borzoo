﻿using Borzoo.GraphQL.Types;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public class ZevereMutation : ObjectGraphType
    {
        public ZevereMutation(IQueryResolver resolver)
        {
            Name = nameof(ZevereMutation);

            Field<UserType>("createUser",
                "Create a new user", new QueryArguments(
                    new QueryArgument<NonNullGraphType<UserInput>> {Name = "user"}
                ),
                resolver.CreateUserAsync
            );

            Field<TaskListType>("createTaskList",
                "Create a new task list for user", new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "owner",
                        Description = "Username of owner"
                    },
                    new QueryArgument<NonNullGraphType<TaskListInput>> {Name = "list"}
                ),
                resolver.CreateTaskListAsync
            );
        }
    }
}