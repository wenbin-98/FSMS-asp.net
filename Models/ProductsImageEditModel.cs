namespace FSMS_asp.net.Models
{
    public class ProductsImageEditModel : ProductsImageUploadModel
    {
        public int Id { get; set; }
        public string? ExistingImage { get; set; }
    }
}
