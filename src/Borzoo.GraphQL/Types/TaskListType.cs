using Borzoo.Data.Abstractions.Entities;
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

            Field(_ => _.CreatedAt)
                .Description("List creation date");

            Field<ListGraphType<TaskItemType>>(
                "tasks",
                "Task items in this list",
                resolve: queryResolver.GetTaskItemsForListAsync
            );
        }
    }
}