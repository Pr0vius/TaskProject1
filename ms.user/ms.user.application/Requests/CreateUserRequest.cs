using System.ComponentModel.DataAnnotations;

namespace ms.user.application.Requests
{
    public class CreateUserRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public string Username { get; set; }
    }
}
