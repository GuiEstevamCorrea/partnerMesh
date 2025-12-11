using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.GetBusinessTypeById.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.GetBusinessTypeById;

public class GetBusinessTypeByIdUseCase : IGetBusinessTypeByIdUseCase
{
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;

    public GetBusinessTypeByIdUseCase(
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository)
    {
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<GetBusinessTypeByIdResult> ExecuteAsync(Guid businessTypeId, Guid userId)
    {
        try
        {
            // Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null || !currentUser.Active)
            {
                return GetBusinessTypeByIdResult.Failure("Usuário atual não encontrado ou inativo.");
            }

            // Verificar permissões - AdminGlobal, AdminVetor e Operador podem consultar tipos de negócio
            var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                               currentUser.Permission.HasFlag(PermissionEnum.AdminVetor) ||
                               currentUser.Permission.HasFlag(PermissionEnum.Operador);

            if (!hasPermission)
            {
                return GetBusinessTypeByIdResult.Failure("Usuário não tem permissão para acessar dados de tipos de negócio.");
            }

            // Buscar o tipo de negócio
            var businessType = await _businessTypeRepository.GetByIdAsync(businessTypeId);
            if (businessType == null)
            {
                return GetBusinessTypeByIdResult.NotFound();
            }

            // Converter para DTO e retornar
            var businessTypeDto = BusinessTypeDetailDto.FromEntity(businessType);
            return GetBusinessTypeByIdResult.Success(businessTypeDto);
        }
        catch (Exception ex)
        {
            return GetBusinessTypeByIdResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}