using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models.Login
{
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
