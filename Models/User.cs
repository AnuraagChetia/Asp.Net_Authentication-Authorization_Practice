using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Practice_Application.Models
{
    public class User
    {
        public int Id { get; set;}
        public required string Name { get; set;}
        [EmailAddress]
        public required string Email { get; set;}
        public required string Password { get; set; }
    }
}
