using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmViewModel
    {
        [Required(ErrorMessage = "E-mail is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
    }
}
