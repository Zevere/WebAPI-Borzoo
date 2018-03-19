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
                .Description("Title of the list");

            Field(_ => _.CreatedAt)
                .Description("List creation date");

            Field<ListGraphType<TaskItemType>>("tasks", "task items", null, queryResolver.GetTaskItemsForListAsync);
        }
    }
}