using MedicamentsAggregator.Service.DataLayer.Tables;
using Microsoft.EntityFrameworkCore;

namespace MedicamentsAggregator.Service.DataLayer.Context
{
    public class MedicamentsAggregatorContext : DbContext
    {
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<PharmacyMedicamentLink> PharmacyMedicamentLinks { get; set; }
        public MedicamentsAggregatorContext(DbContextOptions<MedicamentsAggregatorContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PharmacyMedicamentLink>()
                .HasIndex(p => new { p.MedicamentId, p.PharmacyId })
                .IsUnique();
        }
    }
}