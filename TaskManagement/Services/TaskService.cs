using TaskManagement.Models;

namespace TaskManagement.Services
{
    public class TaskService
    {
        private readonly List<Models.TaskItem> _tasks = new();

        public IEnumerable<Models.TaskItem> GetAll() =>
            _tasks.OrderByDescending(t => t.IsFavorite).ThenBy(t => t.Name);

        public Models.TaskItem? GetById(Guid id) =>
            _tasks.FirstOrDefault(t => t.Id == id);

        public Models.TaskItem Add(Models.TaskItem task)
        {
            task.Id = Guid.NewGuid();
            _tasks.Add(task);
            return task;
        }

        public bool Update(Guid id, Models.TaskItem updated)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;

            task.Name = updated.Name;
            task.taskdate = updated.taskdate;
            task.Column = updated.Column;
            task.IsFavorite = updated.IsFavorite;
            return true;
        }

        public bool Delete(Guid id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;
            _tasks.Remove(task);
            return true;
        }
    }
}
