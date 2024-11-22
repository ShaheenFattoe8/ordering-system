using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fattoe_Shaheen_HW5.Models
{
    public class AppUser : IdentityUser
    {
        // Required first name
        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // Required last name
        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}