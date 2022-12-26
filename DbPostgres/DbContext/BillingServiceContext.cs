using DbPostgres.Models;
using Microsoft.EntityFrameworkCore;

namespace DbPostgres.DbContext;

public class BillingServiceContext : Microsoft.EntityFrameworkCore.DbContext
{
    public BillingServiceContext(DbContextOptions<BillingServiceContext> options) : base(options)
    {
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<CoinModel> Coins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder
        //     .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoinModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("coins_pkey");
            entity.ToTable("coins", "public");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OwnerName).HasColumnName("owner_name");
            entity.Property(e => e.History)
                .HasDefaultValueSql("'{}'::text[]")
                .HasColumnName("history");
            entity.Property(e => e.HistoryLength)
                .HasDefaultValueSql("0")
                .HasColumnName("history_length");

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.Coins)
                .HasForeignKey(d => d.OwnerName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("coins_owner_name_fkey");
        });

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity
                .HasKey(e => e.Name).HasName("users_pkey");
            entity.ToTable("users", "public");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Amount).HasColumnName("amount").HasDefaultValueSql("0");
            entity.Property(e => e.Rating).HasColumnName("rating").HasDefaultValueSql("0");
        });
    }
}