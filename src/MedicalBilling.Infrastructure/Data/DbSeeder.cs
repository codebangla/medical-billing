using Microsoft.EntityFrameworkCore;
using MedicalBilling.Domain.Entities;
using MedicalBilling.Infrastructure.Data;

namespace MedicalBilling.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // 1. Seed Sellers
        if (!await context.Sellers.AnyAsync())
        {
            var sellers = new List<Seller>
            {
                new Seller { Name = "Dr. Schmidt Medical Center", Email = "schmidt@medical.de", LicenseNumber = "LIC-2024-001", Specialty = "Cardiology", CreatedAt = DateTime.UtcNow },
                new Seller { Name = "Dr. Müller Clinic", Email = "mueller@clinic.de", LicenseNumber = "LIC-2024-002", Specialty = "General Practice", CreatedAt = DateTime.UtcNow },
                new Seller { Name = "Dr. Weber Orthopedics", Email = "weber@ortho.de", LicenseNumber = "LIC-2024-003", Specialty = "Orthopedics", CreatedAt = DateTime.UtcNow }
            };
            await context.Sellers.AddRangeAsync(sellers);
            await context.SaveChangesAsync();
        }

        // 2. Seed Products
        if (!await context.Products.AnyAsync())
        {
            var sellers = await context.Sellers.ToListAsync();
            if (sellers.Any())
            {
                var products = new List<Product>
                {
                    // Cardiology (Seller 0)
                    new Product { SellerId = sellers[0].Id, ServiceCode = "CARD-001", ServiceName = "ECG Examination", Description = "Standard 12-lead electrocardiogram", UnitPrice = 85.50m, EBMCode = "13250", CreatedAt = DateTime.UtcNow },
                    new Product { SellerId = sellers[0].Id, ServiceCode = "CARD-002", ServiceName = "Echocardiography", Description = "Ultrasound examination of the heart", UnitPrice = 150.00m, EBMCode = "13545", CreatedAt = DateTime.UtcNow },
                    // GP (Seller 1)
                    new Product { SellerId = sellers[1].Id, ServiceCode = "GP-001", ServiceName = "General Consultation", Description = "Standard medical consultation", UnitPrice = 45.00m, EBMCode = "03000", CreatedAt = DateTime.UtcNow },
                    new Product { SellerId = sellers[1].Id, ServiceCode = "GP-002", ServiceName = "Blood Test", Description = "Complete blood count", UnitPrice = 25.50m, EBMCode = "32120", CreatedAt = DateTime.UtcNow },
                    // Ortho (Seller 2)
                    new Product { SellerId = sellers[2].Id, ServiceCode = "ORTHO-001", ServiceName = "X-Ray Examination", Description = "Digital X-ray imaging", UnitPrice = 65.00m, EBMCode = "34220", CreatedAt = DateTime.UtcNow },
                    new Product { SellerId = sellers[2].Id, ServiceCode = "ORTHO-002", ServiceName = "Physical Therapy", Description = "30-minute physiotherapy", UnitPrice = 55.00m, EBMCode = "30420", CreatedAt = DateTime.UtcNow }
                };
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

        // 3. Seed Patients
        if (!await context.Patients.AnyAsync())
        {
            var patients = new List<Patient>
            {
                new Patient { FirstName = "Hans", LastName = "Müller", DateOfBirth = new DateTime(1975, 5, 15), InsuranceNumber = "INS-1234567890", InsuranceProvider = "AOK Bayern", ContactInfo = "hans.mueller@email.de", CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Anna", LastName = "Schmidt", DateOfBirth = new DateTime(1982, 8, 22), InsuranceNumber = "INS-2345678901", InsuranceProvider = "TK Techniker", ContactInfo = "anna.schmidt@email.de", CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Peter", LastName = "Weber", DateOfBirth = new DateTime(1990, 3, 10), InsuranceNumber = "INS-3456789012", InsuranceProvider = "Barmer", ContactInfo = "peter.weber@email.de", CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Maria", LastName = "Fischer", DateOfBirth = new DateTime(1968, 11, 30), InsuranceNumber = "INS-4567890123", InsuranceProvider = "DAK", ContactInfo = "maria.fischer@email.de", CreatedAt = DateTime.UtcNow },
                new Patient { FirstName = "Klaus", LastName = "Becker", DateOfBirth = new DateTime(1955, 7, 18), InsuranceNumber = "INS-5678901234", InsuranceProvider = "AOK Nordost", ContactInfo = "klaus.becker@email.de", CreatedAt = DateTime.UtcNow }
            };
            await context.Patients.AddRangeAsync(patients);
            await context.SaveChangesAsync();
        }
        
        // 4. Seed Invoices and Billing Procedures
        if (!await context.Invoices.AnyAsync())
        {
            var patients = await context.Patients.ToListAsync();
            var products = await context.Products.ToListAsync();
            
            if (patients.Any() && products.Any())
            {
                // Create an Invoice for Patient 0
                var invoice = new Invoice
                {
                    PatientId = patients[0].Id,
                    InvoiceNumber = $"INV-{DateTime.Now.Year}-001",
                    InvoiceDate = DateTime.UtcNow.AddDays(-2),
                    Status = "Draft",
                    TotalAmount = 0, // Will recalculate
                    CreatedAt = DateTime.UtcNow
                };
                await context.Invoices.AddAsync(invoice);
                await context.SaveChangesAsync();
                
                // Add Procedures to this Invoice
                var procedures = new List<BillingProcedure>
                {
                    new BillingProcedure
                    {
                        PatientId = patients[0].Id,
                        ProductId = products[0].Id, // ECG
                        InvoiceId = invoice.Id,
                        ProcedureDate = DateTime.UtcNow.AddDays(-2),
                        Quantity = 1,
                        UnitPrice = products[0].UnitPrice,
                        TotalAmount = products[0].UnitPrice * 1,
                        Form3ReferenceNumber = "REF-001",
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    },
                    new BillingProcedure
                    {
                        PatientId = patients[0].Id,
                        ProductId = products[1].Id, // Echo
                        InvoiceId = invoice.Id, // Same invoice
                        ProcedureDate = DateTime.UtcNow.AddDays(-2),
                        Quantity = 1,
                        UnitPrice = products[1].UnitPrice,
                        TotalAmount = products[1].UnitPrice * 1,
                        Form3ReferenceNumber = "REF-002",
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow
                    }
                };
                
                await context.BillingProcedures.AddRangeAsync(procedures);
                
                // Update Invoice Total
                invoice.TotalAmount = procedures.Sum(p => p.TotalAmount);
                context.Invoices.Update(invoice);
                
                await context.SaveChangesAsync();
            }
        }
    }
}
