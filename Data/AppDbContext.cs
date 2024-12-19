using Microsoft.EntityFrameworkCore;
using ucsUpdatedApp.Models;

namespace ucsUpdatedApp.Data
{
    public class AppDbContext : DbContext
    {
        
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            public DbSet<TransactionTable> TransactionTable { get; set; }
            public DbSet<MasterTable> MasterTable { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // MasterTable configuration
                modelBuilder.Entity<MasterTable>(entity =>
                {
                    entity.HasKey(e => e.MasterId); // Primary key
                    entity.Property(e => e.EmployeeId).IsRequired();
                    entity.Property(e => e.Employeename).IsRequired();
                    entity.Property(e => e.FingerPrintData).IsRequired(false);
                    entity.Property(e => e.LastTransactionDate).IsRequired(false);
                });

                // TransactionTable configuration
                modelBuilder.Entity<TransactionTable>(entity =>
                {
                    entity.HasKey(e => e.Id); // Primary key
                    entity.Property(e => e.MasterId).IsRequired();
                    entity.Property(e => e.OpDateTime).IsRequired();
                    entity.Property(e => e.Op).IsRequired();
                    entity.Property(e => e.CheckInMethod).IsRequired();
                    entity.Property(e => e.LatestTransactionDate).IsRequired(false);
                });

                // Define foreign key relationship
                modelBuilder.Entity<TransactionTable>()
                    .HasOne(t => t.Master)
                    .WithMany(m => m.Transactions)
                    .HasForeignKey(t => t.MasterId) // Maps to MasterTable.MasterId
                    .OnDelete(DeleteBehavior.Restrict);
            }
        
    }
}

