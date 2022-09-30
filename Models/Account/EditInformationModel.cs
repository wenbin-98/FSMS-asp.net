using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FSMS_asp.net.Models.Account
{
    public class EditInformationModel
    {
        //Edit personal information
        [Required(ErrorMessage = "Please enter your name.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please enter your address.")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
