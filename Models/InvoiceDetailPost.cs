using System.ComponentModel.DataAnnotations;

namespace FSMS_asp.net.Models
{
    public class InvoiceDetailPost
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal TotalAmount { get; set; }
    }
}
