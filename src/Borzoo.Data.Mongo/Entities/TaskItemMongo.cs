using Borzoo.Data.Abstractions.Entities;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo.Entities
{
    public class TaskItemMongo : TaskItem
    {
        public MongoDBRef ListDbRef
        {
            get => _listDbRef;
            set
            {
                _listDbRef = value;
                ListId = ListDbRef?.Id.AsString;
            }
        }

        private MongoDBRef _listDbRef;

        public static TaskItemMongo FromTaskItem(TaskItem entity) => new TaskItemMongo
        {
            Id = entity.Id,
            DisplayId = entity.DisplayId,
            ListId = entity.ListId,
            Title = entity.Title,
        };
    }
}