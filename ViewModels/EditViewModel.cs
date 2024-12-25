using System.ComponentModel.DataAnnotations;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class EditViewModel
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Field { get; set; } 

        public AppRole? Role {get ; set;}

       
        [DataType(DataType.Password)]
        public string? Password { get; set; }

       
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "password does not match")]
        public string? ConfirmPassword { get; set; }

        public IList<string>? SelectedRoles { get; set; }
    }
}