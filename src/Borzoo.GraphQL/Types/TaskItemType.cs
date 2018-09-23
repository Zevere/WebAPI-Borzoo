using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskItemType : ObjectGraphType<TaskItemDto>
    {
        public TaskItemType()
        {
            Name = "Task";
            Description = "Task item";

            Field(_ => _.Id)
                .Description("Task's ID");

            Field(_ => _.Title)
                .Description("Short title of this task");

            Field(_ => _.Description)
                .Description("Description of this task");

            Field(_ => _.Due, true, typeof(DateGraphType))
                .Description("Due date for this task");

            Field(_ => _.Tags, true, typeof(ListGraphType<StringGraphType>))
                .Description("List of tags associated with this task");

            Field(_ => _.CreatedAt)
                .Description("Task creation date");
        }
    }
}