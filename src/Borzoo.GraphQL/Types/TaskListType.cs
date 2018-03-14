using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL.Types
{
    public class TaskListType : ObjectGraphType<TaskListDto>
    {
        public TaskListType()
        {
            Name = "List";
            Description = "Task list";
            
            Field(_ => _.Id)
                .Description("User's ID");
            
            Field(_ => _.Title)
                .Description("Title of the list");
            
            Field(_ => _.CreatedAt)
                .Description("List creation date");
        }
    }
}