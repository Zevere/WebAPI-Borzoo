using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskListInputType : InputObjectGraphType<TaskListCreationDto>
    {
        public TaskListInputType()
        {
            Name = "ListInput";
            Description = "Input for creating a new task list";

            Field(_ => _.Id)
                .Description("The desired task list name. Names are case insensitive and " +
                             "valid characters are ASCII alphanumeric characters, '_', '.', and '-'.");

            Field(_ => _.Title)
                .Description("Short title of task list");
        }
    }
}