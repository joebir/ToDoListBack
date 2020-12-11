using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoListBack.Models
{
    public class ToDoItem
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public DateTime Created { get; set; }
    }
}
