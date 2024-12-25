using System.ComponentModel.DataAnnotations;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Field { get; set; } = string.Empty;

        public AppRole? Role {get; set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "password does not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<string> SelectedRoles { get; set; } = new List<string>();

        public List<string>? AllRoles { get; set; }  // keeping roles
        public string? Username { get; set; }
    }
}