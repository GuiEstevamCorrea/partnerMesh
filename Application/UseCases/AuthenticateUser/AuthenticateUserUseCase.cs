using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.AuthenticateUser.DTO;
using Domain.Entities;

namespace Application.UseCases.AuthenticateUser;

public sealed class AuthenticateUserUseCase : IAuthenticateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticateUserUseCase(
        IUserRepository userRepository, 
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return AuthenticationResult.Failure("Email e senha são obrigatórios.");
        }

        // Buscar usuário por email
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken);
        if (user is null)
        {
            return AuthenticationResult.Failure("Credenciais inválidas.");
        }

        // Verificar se usuário está ativo
        if (!user.Active)
        {
            return AuthenticationResult.Failure("Usuário inativo.");
        }

        // Verificar senha
        if (!user.PasswordMatches(request.Password))
        {
            return AuthenticationResult.Failure("Credenciais inválidas.");
        }

        // Verificar se usuário tem vetor ativo (exceto admin global)
        if (user.Permission != Domain.ValueTypes.PermissionEnum.AdminGlobal && !user.HasActiveVetor())
        {
            return AuthenticationResult.Failure("Usuário sem vetor ativo.");
        }

        // Obter IDs dos vetores
        var vetorIds = user.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToList();

        // Gerar tokens
        var jwtToken = _tokenService.GenerateJwtToken(
            user.Id, 
            user.Name, 
            user.Email, 
            user.Permission.ToString(), 
            vetorIds);

        var refreshToken = _tokenService.GenerateRefreshToken();

        // Salvar refresh token
        var refreshTokenEntity = new Domain.Entities.RefreshToken(
            refreshToken,
            user.Id,
            DateTime.UtcNow.AddDays(30)); // 30 dias de validade

        await _refreshTokenRepository.SaveAsync(refreshTokenEntity, cancellationToken);

        // Criar informações do usuário
        var userInfo = new UserInfo(
            user.Id,
            user.Name,
            user.Email,
            user.Permission,
            vetorIds);

        return AuthenticationResult.Success(jwtToken, refreshToken, userInfo);
    }
}