using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LedgerLite.Domain.Accounts;
using LedgerLite.Domain.Common;
using LedgerLite.Domain.Transactions;

namespace LedgerLite.Api.Data;

// WHY: DbContext wraps the EF Core mapping and lifecycle. In Phase 1, we use SQLite;
// Phase 5 will replace it with Azure SQL without changing domain code.
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
        modelBuilder.Entity<Transaction>().UseTptMappingStrategy();
        modelBuilder.Entity<Income>().ToTable("Incomes");
        modelBuilder.Entity<Expense>().ToTable("Expenses");

        // WHY: Configure basic keys. Value object mapping (Money, Category) is configured below.
        modelBuilder.Entity<Account>().HasKey(a => a.Id);
        modelBuilder.Entity<Transaction>().HasKey(t => t.Id);

        modelBuilder.Entity<Account>().OwnsOne(a => a.Balance, money =>
        {
            money.Property(m => m.Amount).HasColumnName("BalanceAmount").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3);
        });

        modelBuilder.Entity<Transaction>().OwnsOne(t => t.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        modelBuilder.Entity<Transaction>().OwnsOne(t => t.Category, category =>
        {
            category.Property(c => c.Name).HasColumnName("Category").HasMaxLength(100);
        });

        modelBuilder.Entity<Transaction>()
    .HasOne<Account>()
    .WithMany()
    .HasForeignKey(t => t.AccountId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<Transaction>()
    .HasIndex(t => t.AccountId);
    }
}
