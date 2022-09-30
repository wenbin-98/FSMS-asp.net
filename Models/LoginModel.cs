using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models
{
    public class LoginModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
