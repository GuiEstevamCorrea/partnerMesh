namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(Guid userId, string name, string email, string permission, List<Guid> vetorIds);
    string GenerateRefreshToken();
}