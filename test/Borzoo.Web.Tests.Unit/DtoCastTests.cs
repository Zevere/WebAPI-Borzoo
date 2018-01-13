using System;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Web.Models.Task;
using Xunit;

namespace Borzoo.Web.Tests.Unit
{
    public class DtoCastTests
    {
        [Fact]
        public void Should_Cast_TaskPrettyDto()
        {
            const string id = "id";
            const string title = "test task";

            var task = new UserTask(title)
            {
                Id = id,
                Due = DateTime.UtcNow.AddSeconds(20),
            };

            var taskPrettyDto = (TaskPrettyDto) task;

            Assert.Equal(id, taskPrettyDto.Id);
            Assert.Equal(title, taskPrettyDto.Title);
            Assert.False(taskPrettyDto.IsDue);
            Assert.Equal("a few seconds", taskPrettyDto.DueIn);
            Assert.Null(taskPrettyDto.Description);
        }

        [Fact]
        public void Should_Cast_TaskPrettyDto_Due_Task()
        {
            const string id = "id";
            const string title = "test task";

            var task = new UserTask(title)
            {
                Id = id,
                Due = DateTime.UtcNow,
            };

            var taskPrettyDto = (TaskPrettyDto) task;

            Assert.Equal(id, taskPrettyDto.Id);
            Assert.Equal(title, taskPrettyDto.Title);
            Assert.True(taskPrettyDto.IsDue);
            Assert.Null(taskPrettyDto.DueIn);
            Assert.Null(taskPrettyDto.Description);
        }

        [Fact]
        public void Should_Cast_TaskPrettyDto_Due_In_Days()
        {
            const string id = "id";
            const string title = "test task";

            var task = new UserTask(title)
            {
                Id = id,
                Due = DateTime.UtcNow.Add(TimeSpan.Parse("1:7:34:10")),
            };

            var taskPrettyDto = (TaskPrettyDto) task;

            Assert.Equal(id, taskPrettyDto.Id);
            Assert.Equal(title, taskPrettyDto.Title);
            Assert.False(taskPrettyDto.IsDue);
            Assert.Equal("1 day, 7 hours", taskPrettyDto.DueIn);
            Assert.Null(taskPrettyDto.Description);
        }
    }
}