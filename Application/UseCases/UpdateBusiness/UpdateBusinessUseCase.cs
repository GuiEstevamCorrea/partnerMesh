using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.UpdateBusiness.DTO;
using Domain.Entities;
using Domain.Extensions;

namespace Application.UseCases.UpdateBusiness;

public class UpdateBusinessUseCase : IUpdateBusinessUseCase
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessTypeRepository _businessTypeRepository;

    public UpdateBusinessUseCase(
        IBusinessRepository businessRepository,
        IPartnerRepository partnerRepository,
        IBusinessTypeRepository businessTypeRepository)
    {
        _businessRepository = businessRepository;
        _partnerRepository = partnerRepository;
        _businessTypeRepository = businessTypeRepository;
    }

    public async Task<UpdateBusinessResult> ExecuteAsync(Guid businessId, UpdateBusinessRequest request, Guid userId)
    {
        // Validar se há campos para atualizar
        if (!request.HasAnyFieldToUpdate())
        {
            return UpdateBusinessResult.Failure("Nenhum campo foi informado para atualização");
        }

        // Buscar o negócio existente
        var business = await _businessRepository.GetByIdAsync(businessId);
        if (business == null)
        {
            return UpdateBusinessResult.Failure("Negócio não encontrado");
        }

        // Validar se o negócio não está cancelado
        if (business.Status == Domain.ValueTypes.BusinessStatus.Cancelado)
        {
            return UpdateBusinessResult.Failure("Não é possível atualizar negócios cancelados");
        }

        // Buscar informações do parceiro e tipo de negócio para o DTO
        var partner = await _partnerRepository.GetByIdAsync(business.PartnerId);
        var businessType = await _businessTypeRepository.GetByIdAsync(business.BussinessTypeId);

        // Atualizar apenas os campos permitidos (não críticos)
        bool hasChanges = false;
        string changeInfo = "";

        // Atualizar valor se fornecido
        if (request.Value.HasValue && request.Value.Value != business.Value)
        {
            var oldValue = business.Value;
            business.UpdateValue(request.Value.Value);
            hasChanges = true;
            changeInfo += $"Valor alterado de {oldValue:C} para {request.Value.Value:C}. ";
        }

        // Atualizar observações se fornecidas
        if (!string.IsNullOrEmpty(request.Observations) && request.Observations != business.Observations)
        {
            var oldObservations = business.Observations;
            business.UpdateObservations(request.Observations);
            hasChanges = true;
            changeInfo += $"Observações alteradas. ";
        }

        // Se não houve mudanças efetivas, retornar sem salvar
        if (!hasChanges)
        {
            return UpdateBusinessResult.Failure("Nenhuma alteração foi detectada nos campos informados");
        }

        // Salvar alterações
        await _businessRepository.UpdateAsync(business);

        // Preparar resposta
        var businessDto = new UpdatedBusinessDto
        {
            Id = business.Id,
            PartnerId = business.PartnerId,
            PartnerName = partner?.Name ?? "Partner não encontrado",
            BusinessTypeId = business.BussinessTypeId,
            BusinessTypeName = businessType?.Name ?? "Tipo não encontrado",
            Value = business.Value,
            Status = business.Status.ToLegacyString(),
            Date = business.Date,
            Observations = business.Observations,
            CreatedAt = business.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        // IMPORTANTE: Não recalcula comissão após criado (regra do UC-51)
        var commissionInfo = "Comissões não foram recalculadas conforme regra de negócio. " + changeInfo.Trim();

        return UpdateBusinessResult.Success(
            businessDto, 
            commissionRecalculated: false, 
            commissionInfo: commissionInfo
        );
    }
}