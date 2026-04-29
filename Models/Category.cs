using System.Collections.Generic;

namespace TaskManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }= new List<TaskItem>();
    }
}