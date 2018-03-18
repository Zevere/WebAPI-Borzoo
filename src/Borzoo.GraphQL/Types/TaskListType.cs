using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskListType : ObjectGraphType<TaskListDto>
    {
        public TaskListType(IQueryResolver queryResolver)
        {
            Name = "List";
            Description = "Task list";
            
            Field(_ => _.Id)
                .Description("List's ID");
            
            Field(_ => _.Title)
                .Description("Title of the list");
            
            Field(_ => _.CreatedAt)
                .Description("List creation date");

            Field<ListGraphType<TaskItemType>>("tasks", "task items", null, queryResolver.GetTaskItemsForListAsync);
        }
    }
}