using MeuBolso.API.Identity;
using MeuBolso.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeuBolso.API.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions", "app");

            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.Property(t => t.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasColumnName("description")
                .IsRequired(false)
                .HasMaxLength(255);

            builder.Property(t => t.Type)
                .HasColumnName("type")
                .HasConversion<short>()
                .IsRequired()
                .HasComment("1 = Deposit, 2 = Withdraw");

            builder.Property(t => t.Amount)
                .HasColumnName("amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("now()")
                .IsRequired();

            builder.Property(t => t.PaidOrReceivedAt)
                .HasColumnName("paid_or_received_at")
                .HasColumnType("timestamp without time zone")
                .IsRequired(false);
                
            builder.Property(t => t.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .HasConstraintName("fk_transactions_user")
               .OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(t => t.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();
            
            builder.HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .HasConstraintName("fk_transactions_category")
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("ix_transactions_user");
        }
    }
}