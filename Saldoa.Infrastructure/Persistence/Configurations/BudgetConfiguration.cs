using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Configurations;

public class BudgetConfiguration 
    : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("budgets", "app");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id");

        builder.Property(b => b.WorkspaceId)
            .HasColumnName("workspace_id")
            .IsRequired();

        builder.Property(b => b.CategoryId)
            .HasColumnName("category_id")
            .IsRequired(false);

        builder.Property(b => b.Scope)
            .HasColumnName("scope")
            .HasConversion<short>()
            .IsRequired()
            .HasComment("1 = General, 2 = Category");

        builder.Property(b => b.PeriodStart)
            .HasColumnName("period_start")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(b => b.PeriodEnd)
            .HasColumnName("period_end")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        
        builder.Property(b => b.LimitAmount)
            .HasColumnName("limit_amount")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .HasConstraintName("fk_budget_category")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(b => b.WorkspaceId)
            .HasConstraintName("fk_budget_workspace")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new 
        { 
            b.WorkspaceId, 
            b.CategoryId, 
            b.PeriodStart, 
            b.PeriodEnd 
        })
        .IsUnique();
    }
}
