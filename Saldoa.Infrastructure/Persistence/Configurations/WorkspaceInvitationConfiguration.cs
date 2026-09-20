using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saldoa.Domain.Entities;
using Saldoa.Infrastructure.Identity;

namespace Saldoa.Infrastructure.Persistence.Configurations
{
    public class WorkspaceInvitationConfiguration : IEntityTypeConfiguration<WorkspaceInvitation>
    {
        public void Configure(EntityTypeBuilder<WorkspaceInvitation> builder)
        {
            builder.ToTable("workspace_invitation", "app");

            builder.HasKey(x => x.Id);

            builder.Property(w => w.Id)
                .HasColumnName("id");

            builder.Property(w => w.WorkspaceId)
                .HasColumnName("workspace_id")
                .IsRequired();

            builder.Property(w => w.Email)
                .HasColumnName("email")
                .IsRequired();

            builder.Property(w => w.InvitedByUsername)
                .HasColumnName("invited_by_username")
                .IsRequired();

            builder.Property(w => w.TokenHash)
                .HasColumnName("token_hash")
                .IsRequired();

            builder.Property(w => w.Role)
                .HasColumnName("role")
                .HasConversion<short>()
                .IsRequired()
                .HasComment("1 = Member, 2 = Owner, 3 = ReadOnly");

            builder.Property(w => w.Status)
                .HasColumnName("workspace_invitation_status")
                .HasConversion<short>()
                .IsRequired()
                .HasComment("1 = Pending, 2 = Accepted, 3 = Declined, 4 = Expired, 5 = Revoked");

            builder.Property(w => w.AcceptedByUserId)
                .HasColumnName("accepted_by_user_id");

            builder.Property(w => w.InvitedByUserId)
                .HasColumnName("invited_by_user_id")
                .IsRequired();

            builder.Property(w => w.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.Property(w => w.AcceptedAt)
                .HasColumnName("accepted_at")
                .HasColumnType("timestamp with time zone");

            builder.Property(w => w.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            builder.HasOne<Workspace>()
                .WithMany()
                .HasForeignKey(u => u.WorkspaceId)
                .HasConstraintName("fk_workspace_invitations_workspace")
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(u => u.InvitedByUserId)
                .HasConstraintName("fk_workspace_invitations_invited_by_user")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(u => u.AcceptedByUserId)
                .HasConstraintName("fk_workspace_invitations_accepted_by_user")
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(w => w.TokenHash)
                .IsUnique()
                .HasDatabaseName("ux_workspace_invitations_token_hash");

            builder.HasIndex(i => new { i.WorkspaceId, i.Email })
                .IsUnique()
                .HasFilter("workspace_invitation_status = 1")
                .HasDatabaseName("ux_workspace_invitations_pending_email");
        }
    }
}
