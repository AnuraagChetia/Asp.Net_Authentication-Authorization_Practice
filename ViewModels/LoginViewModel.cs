using System.ComponentModel.DataAnnotations;

namespace Practice_Application.ViewModels
{
    public class LoginViewModel
    {
  
        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
