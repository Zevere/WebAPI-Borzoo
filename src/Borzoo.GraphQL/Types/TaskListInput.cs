using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskListInput : InputObjectGraphType<TaskListCreationDto>
    {
        public TaskListInput()
        {
            Name = "TaskListInput";
            Description = "Input for creating a new task list";

            Field(_ => _.Name)
                .Description("The desired task list name. Names are case insensitive and " +
                             "valid characters are ASCII alphanumeric characters, _, ., and -.");
            Field(_ => _.Title).Description("Title of task list");
        }
    }
}