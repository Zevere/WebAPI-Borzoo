using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    /// <summary>
    /// Represents input type for the mutation of creating a task list
    /// </summary>
    public class TaskListInputType : InputObjectGraphType<TaskListCreationDto>
    {
        /// <inheritdoc />
        public TaskListInputType()
        {
            Name = "ListInput";
            Description = "Input for creating a new task list";

            Field(_ => _.Id, nullable: true)
                .Description("The desired task list name");

            Field(_ => _.Title)
                .Description("Short title of task list");

            Field(_ => _.Description, nullable: true)
                .Description("List's descriptions");

            Field(_ => _.Collaborators, type: typeof(ListGraphType<NonNullGraphType<StringGraphType>>))
                .Description("User IDs of the list collaborators");

            Field(_ => _.Tags, type: typeof(ListGraphType<NonNullGraphType<StringGraphType>>))
                .Description("List's tags");
        }
    }
}
