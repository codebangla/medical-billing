using Microsoft.EntityFrameworkCore;
using MedicalBilling.Domain.Entities;

namespace MedicalBilling.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for Medical Billing application
/// Configured with SQL Server, optimized indexing, and security features
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Seller> Sellers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<BillingProcedure> BillingProcedures { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seller configuration
        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Specialty).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Indexes for performance
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
            
            // One-to-many relationship with Products
            entity.HasMany(e => e.Products)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.EBMCode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Indexes
            entity.HasIndex(e => e.SellerId);
            entity.HasIndex(e => e.ServiceCode);
        });
        
        // Patient configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.InsuranceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.InsuranceProvider).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactInfo).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Indexes
            entity.HasIndex(e => e.InsuranceNumber).IsUnique();
            entity.HasIndex(e => new { e.LastName, e.FirstName });
        });
        
        // BillingProcedure configuration
        modelBuilder.Entity<BillingProcedure>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProcedureDate).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Form3ReferenceNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Indexes for performance
            entity.HasIndex(e => new { e.PatientId, e.ProcedureDate });
            entity.HasIndex(e => e.Form3ReferenceNumber).IsUnique();
            entity.HasIndex(e => e.Status);
            
            // Relationships
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.BillingProcedures)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Product)
                .WithMany(p => p.BillingProcedures)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.BillingProcedures)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.InvoiceDate).IsRequired();
            entity.Property(e => e.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Draft");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Indexes
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
            entity.HasIndex(e => new { e.PatientId, e.InvoiceDate });
            entity.HasIndex(e => e.Status);
            
            // Relationship
            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically update timestamps
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Property("CreatedAt") != null)
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                if (entry.Property("UpdatedAt") != null)
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Property("UpdatedAt") != null)
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}
