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
                .Description("task's ID");
            
            Field(_ => _.Title)
                .Description("Title of the task");
            
            Field(_ => _.Description)
                .Description("Description of the task");
            
//            Field(_ => _.Due)
//                .Description("Due date for the task, if exists");
            
            Field(_ => _.CreatedAt)
                .Description("Task creation date");
        }
    }
}