using System.ComponentModel.DataAnnotations;

namespace ms.auth.application.Requests
{
    public class AccountRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
