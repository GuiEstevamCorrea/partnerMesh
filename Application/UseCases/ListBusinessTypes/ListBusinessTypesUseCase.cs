using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListBusinessTypes.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.ListBusinessTypes;

public class ListBusinessTypesUseCase : IListBusinessTypesUseCase
{
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;

    public ListBusinessTypesUseCase(
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository)
    {
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<ListBusinessTypesResult> ExecuteAsync(ListBusinessTypesRequest request, Guid userId)
    {
        try
        {
            // Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null || !currentUser.Active)
            {
                return ListBusinessTypesResult.Failure("Usuário atual não encontrado ou inativo.");
            }

            // Verificar permissões - AdminGlobal, AdminVetor e Operador podem listar tipos de negócio
            var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                               currentUser.Permission.HasFlag(PermissionEnum.AdminVetor) ||
                               currentUser.Permission.HasFlag(PermissionEnum.Operador);

            if (!hasPermission)
            {
                return ListBusinessTypesResult.Failure("Usuário não tem permissão para listar tipos de negócio.");
            }

            // Validar parâmetros de paginação
            if (request.Page < 1)
            {
                return ListBusinessTypesResult.Failure("Número da página deve ser maior que zero.");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ListBusinessTypesResult.Failure("Tamanho da página deve estar entre 1 e 100.");
            }

            // Buscar tipos de negócio com filtros e paginação
            var (businessTypes, totalCount) = await _businessTypeRepository.GetFilteredAsync(request);

            // Converter para DTO
            var businessTypeDtos = businessTypes.Select(BusinessTypeListDto.FromEntity);

            // Calcular informações de paginação
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var paginationInfo = new PaginationInfo
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };

            return ListBusinessTypesResult.Success(businessTypeDtos, paginationInfo);
        }
        catch (Exception ex)
        {
            return ListBusinessTypesResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}