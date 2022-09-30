using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSMS_asp.net.Models.Delivery_Order
{
    [Table("DOrderDetails")]
    public class DOrderDetailsModel
    {
        [Key]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }
        public string ProductName { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public int DOrdersId { get; set; }
        [ForeignKey("DOrdersId")]
        public virtual DOrdersModel DOrders { get; set; }
    }
}
