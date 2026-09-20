using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Domain.Entities;

namespace Saldoa.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories", "app");

            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id)
                .HasColumnName("id");

            builder.Property(c => c.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(x => x.NormalizedName)
                .HasColumnName("normalized_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(c => c.Color)
                .HasColumnName("color")
                .IsRequired(false)
                .HasMaxLength(12);

            builder.Property(x => x.WorkspaceId)
                .HasColumnName("workspace_id")
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasOne<Workspace>()
               .WithMany()
               .HasForeignKey(c => c.WorkspaceId)
               .HasConstraintName("fk_categories_workspace")
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => new { c.WorkspaceId, c.NormalizedName })
                .IsUnique()
                .HasDatabaseName("ux_categories_workspace_normalized_name");
        }
    }
}