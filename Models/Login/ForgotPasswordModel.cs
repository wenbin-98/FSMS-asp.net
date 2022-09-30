using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models.Login
{
    public class ForgotPasswordModel
    {
        [Required, EmailAddress, Display(Name = "Email Address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}
