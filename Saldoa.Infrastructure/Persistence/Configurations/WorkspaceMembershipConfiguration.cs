using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Domain.Entities;
using Saldoa.Infrastructure.Identity;

namespace Saldoa.Infrastructure.Persistence.Configurations
{
    public class WorkspaceMembershipConfiguration : IEntityTypeConfiguration<WorkspaceMembership>
    {
        public void Configure(EntityTypeBuilder<WorkspaceMembership> builder)
        {
            builder.ToTable("workspace_memberships", "app");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .HasColumnName("id");

            builder.Property(w => w.WorkspaceId)
                .HasColumnName("workspace_id")
                .IsRequired();

            builder.Property(w => w.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(w => w.Role)
                .HasColumnName("role")
                .HasConversion<short>()
                .IsRequired()
                .HasComment("1 = Member, 2 = Owner, 3 = ReadOnly");

            builder.Property(w => w.JoinedAt)
                .HasColumnName("joined_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .HasConstraintName("fk_workspace_memberships_user")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Workspace>(m => m.Workspace)
                .WithMany(w => w.Memberships)
                .HasForeignKey(m => m.WorkspaceId)
                .HasConstraintName("fk_workspace_memberships_workspace")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => new { w.WorkspaceId, w.UserId })
                .IsUnique()
                .HasDatabaseName("ux_workspace_member_user");
        }
    }
}
