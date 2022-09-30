using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models
{
    public class CustomersModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Phone No.")]
        [DataType(DataType.PhoneNumber)]
        public string? HpNo { get; set; }

        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedDateTime { get; set; } = DateTime.Now;
    }
}
