using Saldoa.Application.Auth.Abstractions;
using Saldoa.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Saldoa.API.Infrastructure.Persistence;

namespace Saldoa.API.Auth;

public class RefreshTokenRepository(SaldoaDbContext db) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => db.Set<RefreshToken>().AddAsync(token, ct).AsTask();

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => db.Set<RefreshToken>().SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}