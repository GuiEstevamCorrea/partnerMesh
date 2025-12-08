using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.AuthenticateUser.DTO;
using Application.UseCases.RefreshToken.DTO;
using Domain.Entities;

namespace Application.UseCases.RefreshToken;

public sealed class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Validar entrada
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return RefreshTokenResult.Failure("Refresh token é obrigatório.");
        }

        // Buscar refresh token
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshToken is null)
        {
            return RefreshTokenResult.Failure("Refresh token inválido.");
        }

        // Verificar se o refresh token é válido
        if (!refreshToken.IsValid())
        {
            return RefreshTokenResult.Failure("Refresh token expirado ou revogado.");
        }

        // Buscar usuário associado
        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (user is null)
        {
            return RefreshTokenResult.Failure("Usuário não encontrado.");
        }

        // Verificar se usuário ainda está ativo
        if (!user.Active)
        {
            return RefreshTokenResult.Failure("Usuário inativo.");
        }

        // Verificar se usuário tem vetor ativo (exceto admin global)
        if (user.Permission != Domain.ValueTypes.PermissionEnum.AdminGlobal && !user.HasActiveVetor())
        {
            return RefreshTokenResult.Failure("Usuário sem vetor ativo.");
        }

        // Marcar refresh token atual como usado
        refreshToken.MarkAsUsed();
        await _refreshTokenRepository.SaveAsync(refreshToken, cancellationToken);

        // Obter IDs dos vetores
        var vetorIds = user.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToList();

        // Gerar novos tokens
        var newJwtToken = _tokenService.GenerateJwtToken(
            user.Id,
            user.Name,
            user.Email,
            user.Permission.ToString(),
            vetorIds);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Salvar novo refresh token
        var newRefreshTokenEntity = new Domain.Entities.RefreshToken(
            newRefreshToken,
            user.Id,
            DateTime.UtcNow.AddDays(30)); // 30 dias de validade

        await _refreshTokenRepository.SaveAsync(newRefreshTokenEntity, cancellationToken);

        // Criar informações do usuário
        var userInfo = new UserInfo(
            user.Id,
            user.Name,
            user.Email,
            user.Permission,
            vetorIds);

        return RefreshTokenResult.Success(newJwtToken, newRefreshToken, userInfo);
    }
}