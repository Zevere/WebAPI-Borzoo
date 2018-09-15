using Borzoo.GraphQL.Models;
using GraphQL.Types;
using Newtonsoft.Json.Linq;

namespace Borzoo.GraphQL.Types
{
    public class TaskItemInputType : InputObjectGraphType<TaskItemCreationDto>
    {
        public TaskItemInputType()
        {
            Name = "TaskInput";
            Description = "Input for creating a new task";

            Field(_ => _.Id)
                .Description("The desired task list name. Names are case insensitive and " +
                             "valid characters are ASCII alphanumeric characters, _, ., and -.");

            Field(_ => _.Title)
                .Description("Title of task list");

            Field(_ => _.Description, true);

            Field(_ => _.Due, true, typeof(DateGraphType));

            Field(_ => _.Tags, true, typeof(ListGraphType<StringGraphType>));
        }
    }
}