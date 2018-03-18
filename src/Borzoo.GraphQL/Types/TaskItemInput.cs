using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskItemInput : InputObjectGraphType<TaskItemCreationDto>
    {
        public TaskItemInput()
        {
            Name = "TaskInput";
            Description = "Input for creating a new task";

            Field(_ => _.Name)
                .Description("The desired task list name. Names are case insensitive and " +
                             "valid characters are ASCII alphanumeric characters, _, ., and -.");

            Field(_ => _.Title)
                .Description("Title of task list");

            Field(_ => _.Description);

            Field(_ => _.Due, true, typeof(DateGraphType));

            Field(_ => _.Tags, true);
        }
    }
}