using Microsoft.EntityFrameworkCore;

namespace RuleEngine.Sqlite.Data;

/// <summary>
/// Entity Framework DbContext for RuleEngine
/// </summary>
public class RuleDbContext : DbContext
{
    public RuleDbContext(DbContextOptions<RuleDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Rules table
    /// </summary>
    public DbSet<RuleEntity> Rules { get; set; } = null!;

    /// <summary>
    /// Rule versions table
    /// </summary>
    public DbSet<RuleVersionEntity> RuleVersions { get; set; } = null!;

    /// <summary>
    /// Rule parameters table
    /// </summary>
    public DbSet<RuleParameterEntity> RuleParameters { get; set; } = null!;

    /// <summary>
    /// Rule execution audits table
    /// </summary>
    public DbSet<RuleExecutionAuditEntity> RuleExecutionAudits { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Rules table
        modelBuilder.Entity<RuleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Tags).HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Status);
        });

        // Configure RuleVersions table
        modelBuilder.Entity<RuleVersionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.RuleId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PredicateExpression).IsRequired();
            entity.Property(e => e.ResultExpression).IsRequired();
            entity.Property(e => e.Language).IsRequired().HasMaxLength(20).HasDefaultValue("csharp");
            entity.Property(e => e.Metadata);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();

            entity.HasIndex(e => new { e.RuleId, e.Version }).IsUnique();
            entity.HasIndex(e => new { e.RuleId, e.IsActive });

            entity.HasOne<RuleEntity>()
                .WithMany()
                .HasForeignKey(e => e.RuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure RuleParameters table
        modelBuilder.Entity<RuleParameterEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.RuleId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value);

            entity.HasIndex(e => new { e.RuleId, e.Name }).IsUnique();

            entity.HasOne<RuleEntity>()
                .WithMany()
                .HasForeignKey(e => e.RuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure RuleExecutionAudits table
        modelBuilder.Entity<RuleExecutionAuditEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.RuleId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Input);
            entity.Property(e => e.Output);
            entity.Property(e => e.ErrorMessage);
            entity.Property(e => e.Duration).IsRequired();
            entity.Property(e => e.ExecutedAt).IsRequired();

            entity.HasIndex(e => e.RuleId);
            entity.HasIndex(e => e.ExecutedAt);
        });
    }
}