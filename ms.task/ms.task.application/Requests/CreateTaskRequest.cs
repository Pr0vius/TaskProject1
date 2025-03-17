using System.ComponentModel.DataAnnotations;

namespace ms.task.application.Requests
{
    public class CreateTaskRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

    }
}
