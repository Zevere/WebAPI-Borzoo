using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    /// <summary>
    /// Represents input type for creating a new task
    /// </summary>
    public class TaskItemInputType : InputObjectGraphType<TaskItemCreationDto>
    {
        /// <inheritdoc />
        public TaskItemInputType()
        {
            Name = "TaskInput";
            Description = "Input for creating a new task";

            Field(_ => _.Id, nullable: true)
                .Description("The desired task list name. Names are case insensitive and " +
                             "valid characters are ASCII alphanumeric characters, '_', '.', and '-'.");

            Field(_ => _.Title)
                .Description("Short title of the new task");

            Field(_ => _.Description, true)
                .Description("Description of the new task");

            Field(_ => _.Due, true, typeof(DateGraphType))
                .Description("Due data of the new task");

            Field(_ => _.Tags, true, typeof(ListGraphType<StringGraphType>))
                .Description("List of tags on the new task item");
        }
    }
}
