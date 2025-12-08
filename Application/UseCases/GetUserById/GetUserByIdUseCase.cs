using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetUserById.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetUserById;

public sealed class GetUserByIdUseCase : IGetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public GetUserByIdUseCase(
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<GetUserByIdResult> GetAsync(Guid userId, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return GetUserByIdResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar se o usuário atual está ativo
        if (!currentUser.Active)
        {
            return GetUserByIdResult.Failure("Usuário atual está inativo.");
        }

        // Buscar usuário alvo
        var targetUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (targetUser is null)
        {
            return GetUserByIdResult.NotFound();
        }

        // Verificar se o usuário atual tem permissão para ver o usuário alvo
        var hasPermission = HasPermissionToViewUser(currentUser, targetUser);
        if (!hasPermission)
        {
            return GetUserByIdResult.Failure("Você não tem permissão para visualizar este usuário.");
        }

        // Buscar informações detalhadas dos vetores
        var userDetailInfo = await BuildUserDetailInfo(targetUser, cancellationToken);

        return GetUserByIdResult.Success(userDetailInfo);
    }

    private bool HasPermissionToViewUser(Domain.Entities.User currentUser, Domain.Entities.User targetUser)
    {
        // Admin Global pode ver qualquer usuário
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return true;
        }

        // Admin de Vetor pode ver:
        // 1. Admin Global
        // 2. Usuários do mesmo vetor
        if (currentUser.Permission == PermissionEnum.AdminVetor)
        {
            // Sempre pode ver Admin Global
            if (targetUser.Permission == PermissionEnum.AdminGlobal)
            {
                return true;
            }

            // Pode ver usuários do mesmo vetor
            var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToHashSet();
            var targetUserVetorIds = targetUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
            
            if (targetUserVetorIds.Any(vetorId => currentUserVetorIds.Contains(vetorId)))
            {
                return true;
            }
        }

        // Operador pode ver:
        // 1. Admin Global
        // 2. Usuários do mesmo vetor
        // 3. A si mesmo
        if (currentUser.Permission == PermissionEnum.Operador)
        {
            // Pode ver a si mesmo
            if (currentUser.Id == targetUser.Id)
            {
                return true;
            }

            // Sempre pode ver Admin Global
            if (targetUser.Permission == PermissionEnum.AdminGlobal)
            {
                return true;
            }

            // Pode ver usuários do mesmo vetor
            var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToHashSet();
            var targetUserVetorIds = targetUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
            
            if (targetUserVetorIds.Any(vetorId => currentUserVetorIds.Contains(vetorId)))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<UserDetailInfo> BuildUserDetailInfo(Domain.Entities.User user, CancellationToken cancellationToken)
    {
        // Buscar todos os vetores para enriquecer os dados
        var allVetores = await _vetorRepository.GetAllAsync(cancellationToken);
        var vetorLookup = allVetores.ToDictionary(v => v.Id, v => v);

        // Construir detalhes dos vetores associados
        var userVetorDetails = user.UserVetores.Select(uv =>
        {
            var vetor = vetorLookup.TryGetValue(uv.VetorId, out var v) ? v : null;
            
            return new UserVetorDetail(
                uv.VetorId,
                vetor?.Name ?? "Vetor não encontrado",
                vetor?.Email ?? "Email não disponível",
                vetor?.Active ?? false,
                DateTime.UtcNow, // UserVetor não tem CreatedAt, usando data atual
                uv.Active);
        }).ToList();

        return new UserDetailInfo(
            user.Id,
            user.Name,
            user.Email,
            user.Permission.ToString(),
            user.Active,
            user.CreatedAt,
            userVetorDetails);
    }
}