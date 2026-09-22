using Saldoa.Domain.Enums;
using Saldoa.Domain.Exceptions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Saldoa.Domain.Entities
{
    public class WorkspaceInvitation
    {
        private WorkspaceInvitation() { }
        public WorkspaceInvitation(
            string email, 
            string invitedByUsername, 
            Guid workspaceId, 
            string token, 
            WorkspaceRole role,
            Guid invitedByUserId, 
            DateTimeOffset expiresAt)
        {
            if (workspaceId == Guid.Empty)
                throw new DomainException("Workspace inválido.");


            EnsureValidUserId(invitedByUserId);
            EnsureValidToken(token);

            Id = Guid.CreateVersion7();
            Email = EnsureValidEmail(email);
            InvitedByUsername = EnsureValidInvitedByUsername(invitedByUsername);
            WorkspaceId = workspaceId;
            TokenHash = HashToken(token);
            Role = EnsureValidRole(role);
            Status = WorkspaceInvitationStatus.Pending;
            InvitedByUserId = invitedByUserId;
            CreatedAt = DateTimeOffset.UtcNow;
            ExpiresAt = EnsureValidExpiresAt(expiresAt);
        }
        public Guid Id { get; private set; }
        public string Email { get; private set; } = null!;
        public string InvitedByUsername { get; private set; } = null!;
        public Guid WorkspaceId { get; private set; }
        public string TokenHash { get; private set; } = null!;
        public WorkspaceRole Role { get; private set; }
        public WorkspaceInvitationStatus Status { get; private set; }
        public Guid? AcceptedByUserId { get; private set; }
        public Guid InvitedByUserId { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? AcceptedAt { get; private set; }
        public DateTimeOffset ExpiresAt { get; private set; }

        public void Accept(Guid userId)
        {
            CheckCurrentStatus("aceitar");

            if (DateTimeOffset.UtcNow >= ExpiresAt)
                throw new DomainException("Não foi possível aceitar, o convite já expirou.");

            EnsureValidUserId(userId);
            AcceptedByUserId = userId;
            Status = WorkspaceInvitationStatus.Accepted;
            AcceptedAt = DateTimeOffset.UtcNow;
        }

        public void Decline()
        {
            CheckCurrentStatus("recusar");

            if (DateTimeOffset.UtcNow >= ExpiresAt)
                throw new DomainException("Não foi possível recusar, o convite já expirou.");

            Status = WorkspaceInvitationStatus.Declined;
        }

        public void Revoke(Guid userId)
        {
            CheckCurrentStatus("revogar");

            EnsureValidUserId(userId);

            if(!userId.Equals(InvitedByUserId))
                throw new DomainException("Falha ao revogar convite, usuário inválido.");

            Status = WorkspaceInvitationStatus.Revoked;
        }

        public void Expired()
        {
            CheckCurrentStatus("expirar");

            if (ExpiresAt > DateTimeOffset.UtcNow)
                throw new DomainException("Falha ao expirar convite.");

            Status = WorkspaceInvitationStatus.Expired;
        }

        public static string GenerateToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private static void EnsureValidToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Token inválido.");
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }

        private static string EnsureValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email inválido.");

            if (!Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
                throw new DomainException("Email inválido.");

            return email;
        }

        private static string EnsureValidInvitedByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("Usuário inválido.");

            return username;
        }

        private static WorkspaceRole EnsureValidRole(WorkspaceRole role)
        {
            if (!Enum.IsDefined(role))
                throw new DomainException("Tipo de função inválida.");

            return role;
        }

        private DateTimeOffset EnsureValidExpiresAt(DateTimeOffset expiresAt)
        {
            if(expiresAt <= CreatedAt)
                throw new DomainException("Data de expiração inválida");

            return expiresAt;
        }

        private static void EnsureValidUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("Usuário inválido ou não informado.");
        }

        private void CheckCurrentStatus(string label)
        {
            if (Status != WorkspaceInvitationStatus.Pending)
                throw new DomainException($"Não foi possível {label}, o convite não está mais pendente.");
        }
    }
}
