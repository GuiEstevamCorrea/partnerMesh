using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListVetores.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.ListVetores;

public sealed class ListVetoresUseCase : IListVetoresUseCase
{
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public ListVetoresUseCase(
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<ListVetoresResult> ListAsync(ListVetoresRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        // Buscar usuário atual para verificar permissões
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser is null)
        {
            return ListVetoresResult.Failure("Usuário atual não encontrado.");
        }

        // Verificar se o usuário atual está ativo
        if (!currentUser.Active)
        {
            return ListVetoresResult.Failure("Usuário atual está inativo.");
        }

        // Verificar permissões básicas
        if (currentUser.Permission == PermissionEnum.Operador)
        {
            return ListVetoresResult.Failure("Operadores não têm permissão para listar vetores.");
        }

        // Buscar todos os vetores
        var allVetores = await _vetorRepository.GetAllAsync(cancellationToken);
        
        // Aplicar filtro de escopo baseado na permissão
        var scopedVetores = ApplyScopeFilter(allVetores, currentUser);
        
        // Aplicar filtros de busca
        var filteredVetores = ApplySearchFilters(scopedVetores, request);
        
        // Aplicar paginação
        var pagedVetores = ApplyPagination(filteredVetores.ToList(), request);
        
        // Buscar informações dos usuários para enriquecer os dados
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        
        // Converter para DTO
        var vetorListItems = pagedVetores.Items.Select(vetor => MapToVetorListItem(vetor, allUsers));
        
        var result = new PagedVetores(
            vetorListItems,
            request.Page,
            request.PageSize,
            pagedVetores.TotalCount,
            pagedVetores.TotalPages);

        return ListVetoresResult.Success(result);
    }

    private IEnumerable<Domain.Entities.Vetor> ApplyScopeFilter(IEnumerable<Domain.Entities.Vetor> vetores, Domain.Entities.User currentUser)
    {
        // Admin Global vê todos os vetores
        if (currentUser.Permission == PermissionEnum.AdminGlobal)
        {
            return vetores;
        }

        // Admin de Vetor vê apenas seus próprios vetores
        if (currentUser.Permission == PermissionEnum.AdminVetor)
        {
            var currentUserVetorIds = currentUser.UserVetores.Where(uv => uv.Active).Select(uv => uv.VetorId).ToHashSet();
            
            return vetores.Where(vetor => currentUserVetorIds.Contains(vetor.Id));
        }

        // Por segurança, se não for Admin Global nem Admin de Vetor, retorna vazio
        return Enumerable.Empty<Domain.Entities.Vetor>();
    }

    private IEnumerable<Domain.Entities.Vetor> ApplySearchFilters(IEnumerable<Domain.Entities.Vetor> vetores, ListVetoresRequest request)
    {
        var query = vetores.AsQueryable();

        // Filtro por nome (case insensitive, contém)
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(v => v.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por email (case insensitive, contém)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(v => v.Email.Contains(request.Email, StringComparison.OrdinalIgnoreCase));
        }

        // Filtro por status ativo/inativo
        if (request.Active.HasValue)
        {
            query = query.Where(v => v.Active == request.Active.Value);
        }

        return query;
    }

    private PagedResult<Domain.Entities.Vetor> ApplyPagination(List<Domain.Entities.Vetor> vetores, ListVetoresRequest request)
    {
        var totalCount = vetores.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        
        var items = vetores
            .OrderBy(v => v.Name) // Ordenação padrão por nome
            .ThenBy(v => v.CreatedAt) // Desempate por data de criação
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<Domain.Entities.Vetor>(items, totalCount, totalPages);
    }

    private VetorListItem MapToVetorListItem(Domain.Entities.Vetor vetor, IEnumerable<Domain.Entities.User> allUsers)
    {
        // Calcular estatísticas de usuários
        var usuariosDoVetor = allUsers.Where(u => u.UserVetores.Any(uv => uv.Active && uv.VetorId == vetor.Id)).ToList();
        var totalUsuarios = usuariosDoVetor.Count;
        var usuariosAtivos = usuariosDoVetor.Count(u => u.Active);

        // Calcular estatísticas de parceiros
        var totalParceiros = vetor.Partners.Count;
        var parceirosAtivos = vetor.Partners.Count(p => p.Activve); // Nota: há um typo "Activve" na entidade Partner

        return new VetorListItem(
            vetor.Id,
            vetor.Name,
            vetor.Email,
            vetor.Active,
            vetor.CreatedAt,
            totalUsuarios,
            usuariosAtivos,
            totalParceiros,
            parceirosAtivos);
    }

    private sealed record PagedResult<T>(List<T> Items, int TotalCount, int TotalPages);
}