using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Saldoa.Application.Common.Abstractions;
using Saldoa.Domain.Auth;
using Saldoa.Domain.Entities;
using Saldoa.Infrastructure.Identity;
using System.Reflection;

namespace Saldoa.Infrastructure.Persistence
{
    public class SaldoaDbContext : IdentityUserContext<ApplicationUser, Guid>
    {
        private readonly IWorkspaceContext _workspaceContext;
        private Guid CurrentWorkspaceId =>
            _workspaceContext.WorkspaceId ?? throw new InvalidOperationException("O workspace deve ser definido antes desta consulta.");
        public SaldoaDbContext(DbContextOptions<SaldoaDbContext> options, IWorkspaceContext workspaceContext)
            : base(options) 
        {
            _workspaceContext = workspaceContext;
        }

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
            
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.HasDefaultSchema("app");

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims", "auth");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins", "auth");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens", "auth");

            builder.Entity<Transaction>()
                .HasQueryFilter(t => t.WorkspaceId == CurrentWorkspaceId);

            builder.Entity<Category>()
                .HasQueryFilter(c => c.WorkspaceId == CurrentWorkspaceId);

            builder.Entity<CategoryBudget>()
                .HasQueryFilter(b => b.WorkspaceId == CurrentWorkspaceId);
        }
    }
}