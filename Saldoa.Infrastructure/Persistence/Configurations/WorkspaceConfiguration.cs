using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Domain.Entities;
using Saldoa.Infrastructure.Identity;

namespace Saldoa.Infrastructure.Persistence.Configurations
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.ToTable("workspace", "app");

            builder.HasKey(x => x.Id);

            builder.Property(w => w.Id)
                .HasColumnName("id");

            builder.Property(w => w.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(80);

            builder.Property(w => w.CreatedByUserId)
                .HasColumnName("created_by_user_id")
                .IsRequired();

            builder.Property(w => w.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(u => u.CreatedByUserId)
                .HasConstraintName("fk_workspace_created_by_user")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
