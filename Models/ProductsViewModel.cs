using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models
{
    public class ProductsViewModel : ProductsImageEditModel
    {
        [Required]
        [Display(Name = "Product's Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Required]
        [Display(Name = "Price (RM)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
