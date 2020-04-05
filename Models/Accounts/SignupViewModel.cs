using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class SignupViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(6, ErrorMessage = "Password must be at least six characters long")]
        [Display(Name = "Password")]
        public string Passport { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(6, ErrorMessage = "Password must be at least six characters long")]
        [Compare(nameof(Passport), ErrorMessage = "Confirmation does not match")]
        public string ConfirmPassword { get; set; }     

    }
}
