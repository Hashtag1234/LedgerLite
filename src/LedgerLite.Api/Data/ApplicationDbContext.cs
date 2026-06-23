using Microsoft.EntityFrameworkCore;
using LedgerLite.Domain.Accounts;
using LedgerLite.Domain.Transactions;

namespace LedgerLite.Api.Data;

// WHY: DbContext wraps the EF Core mapping and lifecycle. In Phase 1, we use SQLite;
// Phase 5 replaces it with Azure SQL without changing domain code.
public class ApplicationDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // WHY: Configure the Transaction hierarchy with TPT (Table-Per-Type) inheritance.
        // Income and Expense each get their own table.
        modelBuilder.Entity<Transaction>()
            .HasDiscriminator<string>("TransactionType")
            .HasValue<Income>("Income")
            .HasValue<Expense>("Expense");

        modelBuilder.Entity<Income>().ToTable("Incomes");
        modelBuilder.Entity<Expense>().ToTable("Expenses");

        // WHY: Configure basic keys. Value object mapping (Money, Category) 
        // will be added in Phase 1.5 with proper converters.
        modelBuilder.Entity<Account>().HasKey(a => a.Id);
        modelBuilder.Entity<Transaction>().HasKey(t => t.Id);

        // WHY: For Phase 1, we use parameterless constructors from EF Core's perspective.
        // Configure which constructor to use explicitly for each entity type.
        modelBuilder.Entity<Account>().HasOne<Account>().WithOne().IsRequired(false);
        modelBuilder.Entity<Transaction>().HasOne<Transaction>().WithOne().IsRequired(false);
    }
}
