using MeuBolso.Domain.Auth;

namespace MeuBolso.Application.Auth.Abstractions;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}