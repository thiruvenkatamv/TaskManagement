using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Controllers;
using TaskManagement.Models;
using Xunit;

namespace TaskManagement.Tests
{
    public class TaskControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                //.UseSqlServer("Server=tcp:taskdbround2.database.windows.net,1433;Initial Catalog=TASKDB;Persist Security Info=False;User ID=taskdb;Password=Test@1993;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")
                .UseInMemoryDatabase("TestDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void CanConnectToRealDatabase()
        {
            using var context = GetDbContext();
            Assert.True(context.Database.CanConnect());
        }
        [Fact]
        public async Task GetTasks_ReturnsAllTasks()
        {
            var context = GetDbContext();
            context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Name = "TO DO Task 1", Description = "TO DO Task 1", Column = "To Do" });
            context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Name = "TO DO Task 2", Description = "TO DO Task 2 with FAV", Column = "To Do", IsFavorite = true });
            context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Name = "In Progress Task 1", Description = "In Progress Task 1", Column = "In Progress" });
            context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Name = "In Progress Task 2", Description = "In Progress Task 2 with FAV", Column = "In Progress", IsFavorite = true });
            context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Name = "Done Task 1", Description = "Done Task 1", Column = "Done" });
            context.Tasks.Add(new TaskItem { Id = Guid.NewGuid(), Name = "Done Task 2", Description = "Done Task 2 with FAV", Column = "Done", IsFavorite = true });
            context.SaveChanges();

            var controller = new TaskController(context);

            var result = await controller.GetTasks();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
            Assert.NotEmpty((List<TaskItem>)tasks);
        }

        [Fact]
        public async Task GetTask_WithValidId_ReturnsTask()
        {
            var context = GetDbContext();
            var taskId = Guid.NewGuid();
            context.Tasks.Add(new TaskItem { Id = taskId, Name = "Valid Task", Description = "Valid task description", Column = "To Do" });
            context.SaveChanges();

            var controller = new TaskController(context);

            var result = await controller.GetTask(taskId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var task = Assert.IsType<TaskItem>(okResult.Value);
            Assert.Equal(taskId, task.Id);
            Assert.Equal("Valid task description", task.Description);
        }

        [Fact]
        public async Task GetTask_WithInvalidId_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = new TaskController(context);

            var result = await controller.GetTask(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateTask_AddsTaskAndReturnsOk()
        {
            var context = GetDbContext();
            var controller = new TaskController(context);

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Name = "New Task",
                Description = "This is a new task",
                Column = "To Do",
                taskdate = DateTime.UtcNow,
                IsFavorite = true
            };

            var result = await controller.CreateTask(task);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var created = Assert.IsType<TaskItem>(okResult.Value);
            Assert.Equal("New Task", created.Name);
            Assert.Equal("This is a new task", created.Description);
        }

        [Fact]
        public async Task UpdateTask_ValidUpdate_UpdatesAndReturnsNoContent()
        {
            var context = GetDbContext();
            var taskId = Guid.NewGuid();

            var original = new TaskItem
            {
                Id = taskId,
                Name = "Original Task",
                Description = "Original Description",
                Column = "To Do",
                taskdate = DateTime.UtcNow
            };
            context.Tasks.Add(original);
            context.SaveChanges();

            var controller = new TaskController(context);

            original.Name = "Updated Task";
            original.Description = "Updated Description";
            original.Column = "In Progress";
            original.taskdate = original.taskdate;
            original.IsFavorite = true;

            var result = await controller.UpdateTask(taskId, original);
            Assert.IsType<NoContentResult>(result);
            var updated = await context.Tasks.FindAsync(taskId);
            Assert.Equal("Updated Task", updated.Name);
            Assert.Equal("Updated Description", updated.Description);
            Assert.Equal("In Progress", updated.Column);
            Assert.True(updated.IsFavorite);
        }

        [Fact]
        public async Task DeleteTask_RemovesTaskAndReturnsNoContent()
        {
            var context = GetDbContext();
            var taskId = Guid.NewGuid();

            var task = new TaskItem
            {
                Id = taskId,
                Name = "Task to Delete",
                Description = "Will be deleted",
                Column = "Done"
            };
            context.Tasks.Add(task);
            context.SaveChanges();

            var controller = new TaskController(context);

            var result = await controller.DeleteTask(taskId);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(context.Tasks.Find(taskId));
        }

    }
}
