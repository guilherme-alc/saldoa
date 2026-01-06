using MeuBolso.API.Persistence;
using MeuBolso.Application.Auth.Abstractions;
using MeuBolso.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace MeuBolso.API.Auth;

public class RefreshTokenRepository(MeuBolsoDbContext db) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => db.Set<RefreshToken>().AddAsync(token, ct).AsTask();

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => db.Set<RefreshToken>().SingleOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}