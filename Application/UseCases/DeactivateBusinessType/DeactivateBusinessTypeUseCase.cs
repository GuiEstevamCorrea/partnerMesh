using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.DeactivateBusinessType.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.DeactivateBusinessType;

public class DeactivateBusinessTypeUseCase : IDeactivateBusinessTypeUseCase
{
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBusinessRepository _businessRepository;

    public DeactivateBusinessTypeUseCase(
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository,
        IBusinessRepository businessRepository)
    {
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
        _businessRepository = businessRepository;
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

        // Se está ativo, verificar se pode desativar
        if (businessType.Active)
        {
            // Verificar se existem negócios ativos vinculados a este tipo
            var allBusinesses = await _businessRepository.GetAllAsync();
            var activeBusinessesForType = allBusinesses.Where(b => 
                b.BussinessTypeId == businessTypeId && 
                b.Status != Domain.ValueTypes.BusinessStatus.Cancelado).ToList();
            
            if (activeBusinessesForType.Any())
            {
                throw new ArgumentException($"Não é possível inativar este tipo de negócio. Existem {activeBusinessesForType.Count} negócio(s) ativo(s) vinculado(s) a ele.");
            }

            // Inativar o tipo de negócio
            businessType.Deactivate(userId);
        }
        else
        {
            // Reativar o tipo de negócio
            businessType.Activate(userId);
        }

        // Salvar alterações
        await _businessTypeRepository.UpdateAsync(businessType);

        // Retornar resultado
        return DeactivateBusinessTypeResult.From(businessType);
    }
}