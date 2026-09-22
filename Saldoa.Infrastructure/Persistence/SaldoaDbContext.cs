using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saldoa.Domain.Auth;
using Saldoa.Domain.Entities;
using Saldoa.Infrastructure.Identity;
using System.Reflection;

namespace Saldoa.Infrastructure.Persistence
{
    public class SaldoaDbContext : IdentityUserContext<ApplicationUser, Guid>
    {
        public SaldoaDbContext(DbContextOptions<SaldoaDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<CategoryBudget> CategoryBudgets { get; set; } = null!;
        public DbSet<Workspace> Workspaces { get; set; } = null!;
        public DbSet<WorkspaceMembership> WorkspaceMemberships { get; set; } = null!;
        public DbSet<WorkspaceInvitation> WorkspaceInvitations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.HasDefaultSchema("app");

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims", "auth");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins", "auth");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens", "auth");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}