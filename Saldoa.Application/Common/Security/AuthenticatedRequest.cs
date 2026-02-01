using System.Text.Json.Serialization;

namespace Saldoa.Application.Common.Security;

public class AuthenticatedRequest
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}