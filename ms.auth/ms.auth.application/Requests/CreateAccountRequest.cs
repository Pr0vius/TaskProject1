using System.ComponentModel.DataAnnotations;
namespace ms.auth.application.Requests
{
    public class CreateAccountRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
