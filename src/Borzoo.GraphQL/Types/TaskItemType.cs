using Borzoo.Data.Abstractions.Entities;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    /// <summary>
    /// Represents task item type in the GraphQL schema
    /// </summary>
    public class TaskItemType : ObjectGraphType<TaskItem>
    {
        /// <inheritdoc />
        public TaskItemType()
        {
            Name = "Task";
            Description = "Task item";

            Field(_ => _.DisplayId)
                .Name("id")
                .Description("Task's ID");

            Field(_ => _.Title)
                .Description("Short title of this task");

            Field(_ => _.Description, nullable: true)
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
