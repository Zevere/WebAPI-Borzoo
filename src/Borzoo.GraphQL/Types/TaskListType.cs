﻿using Borzoo.Data.Abstractions.Entities;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskListType : ObjectGraphType<TaskList>
    {
        public TaskListType(IQueryResolver queryResolver)
        {
            Name = "List";
            Description = "Task list";

            Field(_ => _.DisplayId)
                .Name("id")
                .Description("List's ID");

            Field(_ => _.Title)
                .Description("Short title of this list");

            Field<NonNullGraphType<StringGraphType>>(
                "owner", "User ID of this task list's owner"
            );

            Field(_ => _.CreatedAt)
                .Description("List creation date");

            Field(_ => _.Description, nullable: true)
                .Description("Task list descriptions");

            Field(_ => _.ModifiedAt, nullable: true)
                .Name("updatedAt")
                .Description("Last time this list was updated");

            Field<ListGraphType<TaskItemType>>(
                "tasks",
                "Task items in this list",
                resolve: queryResolver.GetTaskItemsForListAsync
            );

            Field(_ => _.Tags, type: typeof(ListGraphType<NonNullGraphType<StringGraphType>>))
                .Description("List's tags");

            Field(_ => _.Collaborators, type: typeof(ListGraphType<NonNullGraphType<StringGraphType>>))
                .Description("User IDs of the list collaborators");
        }
    }
}
