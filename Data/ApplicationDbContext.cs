using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FSMS_asp.net.Models;
using FSMS_asp.net.Models.Quotation;

namespace FSMS_asp.net.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CustomersModel>? CustomersModel { get; set; }
        public DbSet<ProductsModel>? ProductsModel { get; set; }
        public DbSet<InvoicesModel>? InvoicesModel { get; set; }

        public DbSet<InvoiceDetailsModel>? InvoiceDetailsModel { get; set; }

        public DbSet<AccountViewModel> AccountViewModel { get; set; }

        public DbSet<FSMS_asp.net.Models.Quotation.QuotationsModel> QuotationsModel { get; set; }

        public DbSet<FSMS_asp.net.Models.Quotation.QuotationDetailsModel> QuotationDetailsModel { get; set; }

        public DbSet<FSMS_asp.net.Models.Delivery_Order.DOrdersModel> DOrdersModel { get; set; }

        public DbSet<FSMS_asp.net.Models.Delivery_Order.DOrderDetailsModel> DOrderDetailsModel { get; set; }
    }
}