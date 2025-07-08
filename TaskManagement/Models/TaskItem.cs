using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public DateTime taskdate { get; set; }
        public string Column { get; set; } = "To Do"; // Default column
        public bool IsFavorite { get; set; } = false;

    }
}
