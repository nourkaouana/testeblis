using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using testbills.Models;

namespace testbills.Data
{
    public class ApplicationDbContext : IdentityUserContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public DbSet<InvoiceData> Invoices { get; set; }
        public DbSet<TaxDetail> TaxDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=app.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation entre InvoiceData et TaxDetail
            modelBuilder.Entity<InvoiceData>()
                .HasMany(i => i.TaxDetails)
                .WithOne(t => t.InvoiceData)
                .HasForeignKey(t => t.InvoiceDataId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
