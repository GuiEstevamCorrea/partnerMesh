using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.DeactivateBusinessType.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.DeactivateBusinessType;

public class DeactivateBusinessTypeUseCase : IDeactivateBusinessTypeUseCase
{
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;

    public DeactivateBusinessTypeUseCase(
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository)
    {
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<DeactivateBusinessTypeResult> ExecuteAsync(Guid businessTypeId, Guid userId)
    {
        // Validar usuário atual
        var currentUser = await _userRepository.GetByIdAsync(userId);
        if (currentUser == null || !currentUser.Active)
        {
            throw new UnauthorizedAccessException("Usuário não encontrado ou inativo.");
        }

        // Verificar permissões - apenas AdminGlobal e AdminVetor podem inativar tipos de negócio
        var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                           currentUser.Permission.HasFlag(PermissionEnum.AdminVetor);

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("Usuário não tem permissão para inativar tipos de negócio.");
        }

        // Buscar tipo de negócio existente
        var businessType = await _businessTypeRepository.GetByIdAsync(businessTypeId);
        if (businessType == null)
        {
            throw new ArgumentException("Tipo de negócio não encontrado.");
        }

        // Verificar se já está inativo
        if (!businessType.Active)
        {
            throw new ArgumentException("Tipo de negócio já está inativo.");
        }

        // TODO: Em uma implementação futura, aqui verificaríamos se existem negócios ativos 
        // vinculados a este tipo antes de permitir a inativação
        // Exemplo: 
        // var hasActiveBusinesses = await _businessRepository.HasActiveBusinessesForTypeAsync(businessTypeId);
        // if (hasActiveBusinesses)
        // {
        //     throw new ArgumentException("Não é possível inativar um tipo de negócio que possui negócios ativos vinculados.");
        // }

        // Inativar o tipo de negócio
        businessType.Deactivate(userId);

        // Salvar alterações
        await _businessTypeRepository.UpdateAsync(businessType);

        // Retornar resultado
        return DeactivateBusinessTypeResult.From(businessType);
    }
}