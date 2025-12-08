namespace Application.UseCases.AuthenticateUser.DTO;

public sealed record AuthenticationRequest(string Email, string Password);