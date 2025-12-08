using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListUsers.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.ListUsers;

public sealed class ListUsersUseCase : IListUsersUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IVetorRepository _vetorRepository;

    public ListUsersUseCase(
        IUserRepository userRepository,
        IVetorRepository vetorRepository)
    {
        _userRepository = userRepository;
        _vetorRepository = vetorRepository;
    }

    public async Task<ListUsersResult> ListAsync(ListUsersRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return ListUsersResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar se o usuário atual está ativo
        if (!currentUser.Active)
        {
            return ListUsersResult.Failure("Usuário atual está inativo.");
        }

        // Verificar permissões básicas
        if (currentUser.Permission == PermissionEnum.Operador)
        {
            return ListUsersResult.Failure("Operadores não têm permissão para listar usuários.");
        }

        // Buscar todos os usuários
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Aplicar filtro de escopo baseado na permissão
        var scopedUsers = ApplyScopeFilter(allUsers, currentUser);
        
        // Aplicar filtros de busca
        var filteredUsers = ApplySearchFilters(scopedUsers, request);
        
        // Aplicar paginação
        var pagedUsers = ApplyPagination(filteredUsers.ToList(), request);
        
        // Buscar informações dos vetores para enriquecer os dados
        var vetorInfos = await GetVetorInfos(cancellationToken);
        
        // Converter para DTO
        var userListItems = pagedUsers.Items.Select(user => MapToUserListItem(user, vetorInfos));
        
        var result = new PagedUsers(
            userListItems,
            request.Page,
            request.PageSize,
            pagedUsers.TotalCount,
            pagedUsers.TotalPages);

        return ListUsersResult.Success(result);
    }

    private IEnumerable<Domain.Entities.User> ApplyScopeFilter(IEnumerable<Domain.Entities.User> users, Domain.Entities.User currentUser)
    {
        // Admin Global vê todos os usuários
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return users;
        }

        // Admin de Vetor vê apenas usuários do(s) mesmo(s) vetor(es)
        if (currentUser.Permission == PermissionEnum.AdminVetor)
        {
            var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToHashSet();
            
            return users.Where(user =>
            {
                // Admin Global sempre visível (para admins de vetor verem)
                if (user.Permission == PermissionEnum.AdminGlobal)
                {
                    return true;
                }
                
                // Usuários do mesmo vetor
                var userVetorIds = user.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId);
                return userVetorIds.Any(vetorId => currentUserVetorIds.Contains(vetorId));
            });
        }

        // Por segurança, se não for Admin Global nem Admin de Vetor, retorna vazio
        return Enumerable.Empty<Domain.Entities.User>();
    }

    private IEnumerable<Domain.Entities.User> ApplySearchFilters(IEnumerable<Domain.Entities.User> users, ListUsersRequest request)
    {
        var query = users.AsQueryable();

        // Filtro por nome (case insensitive, contém)
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(u => u.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por email (case insensitive, contém)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(u => u.Email.Contains(request.Email, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por permissão
        if (request.Permission.HasValue)
        {
            query = query.Where(u => u.Permission == request.Permission.Value);
        }

        // Filtro por vetor
        if (request.VetorId.HasValue)
        {
            query = query.Where(u => u.UserVetores.Any(uv => uv.Active && uv.VetorId == request.VetorId.Value));
        }

        // Filtro por status ativo/inativo
        if (request.Active.HasValue)
        {
            query = query.Where(u => u.Active == request.Active.Value);
        }

        return query;
    }

    private PagedResult<Domain.Entities.User> ApplyPagination(List<Domain.Entities.User> users, ListUsersRequest request)
    {
        var totalCount = users.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        
        var items = users
            .OrderBy(u => u.Name) // Ordenação padrão por nome
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<Domain.Entities.User>(items, totalCount, totalPages);
    }

    private async Task<Dictionary<Guid, string>> GetVetorInfos(CancellationToken cancellationToken)
    {
        var vetores = await _vetorRepository.GetAllAsync(cancellationToken);
        return vetores.ToDictionary(v => v.Id, v => v.Name);
    }

    private UserListItem MapToUserListItem(Domain.Entities.User user, Dictionary<Guid, string> vetorInfos)
    {
        var vetorInfosList = user.UserVetores
            .Where(uv => uv.Active)
            .Select(uv => new VetorInfo(
                uv.VetorId,
                vetorInfos.TryGetValue(uv.VetorId, out var vetorName) ? vetorName : "Vetor não encontrado"))
            .ToList();

        return new UserListItem(
            user.Id,
            user.Name,
            user.Email,
            user.Permission.ToString(),
            user.Active,
            user.CreatedAt,
            vetorInfosList);
    }

    private sealed record PagedResult<T>(List<T> Items, int TotalCount, int TotalPages);
}