using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListPayments.DTO;
using Application.UseCases.ListPartners.DTO;

namespace Application.UseCases.ListPayments;

public class ListPaymentsUseCase : IListPaymentsUseCase
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessRepository _businessRepository;

    public ListPaymentsUseCase(
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessRepository businessRepository)
    {
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessRepository = businessRepository;
    }

    public async Task<ListPaymentsResult> ExecuteAsync(
        ListPaymentsRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar entrada
            if (!request.IsValidStatus())
            {
                return ListPaymentsResult.Failure("Status inválido.");
            }

            if (!request.IsValidTipoPagamento())
            {
                return ListPaymentsResult.Failure("Tipo de pagamento inválido.");
            }

            if (request.Page < 1)
            {
                return ListPaymentsResult.Failure("Página deve ser maior que zero.");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return ListPaymentsResult.Failure("Tamanho da página deve estar entre 1 e 100.");
            }

            // Buscar pagamentos com filtros
            var (payments, totalCount) = await _commissionRepository.GetPaymentsWithFiltersAsync(
                request.VetorId,
                request.PartnerId,
                request.StartDate,
                request.EndDate,
                request.Status,
                request.TipoPagamento,
                request.SortBy,
                request.SortDirection,
                request.Page,
                request.PageSize,
                cancellationToken);

            // Converter para DTOs
            var paymentDtos = new List<PaymentListDto>();

            foreach (var payment in payments)
            {
                // Buscar dados do parceiro
                var partner = await _partnerRepository.GetByIdAsync(payment.PartnerId, cancellationToken);
                
                // Buscar dados do negócio através da comissão
                var business = await _businessRepository.GetByIdAsync(payment.Comission.BussinessId, cancellationToken);

                var paymentDto = new PaymentListDto
                {
                    Id = payment.Id,
                    ComissionId = payment.ComissionId,
                    PartnerId = payment.PartnerId,
                    PartnerName = partner?.Name ?? "Parceiro não encontrado",
                    TipoPagamento = payment.TipoPagamento,
                    Value = payment.Value,
                    Status = payment.Status,
                    PaidOn = payment.PaidOn,
                    CreatedAt = payment.Comission.CreatedAt,
                    BusinessId = business?.Id ?? Guid.Empty,
                    BusinessDescription = business?.Observations ?? "",
                    BusinessTotalValue = business?.Value ?? 0,
                    BusinessDate = business?.CreatedAt ?? DateTime.MinValue,
                    VetorId = Guid.Empty, // TODO: Implementar quando houver relação direta
                    VetorName = "" // TODO: Implementar quando houver relação direta
                };

                paymentDtos.Add(paymentDto);
            }

            // Criar informações de paginação
            var pagination = new PaginationInfo
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            return ListPaymentsResult.Success(paymentDtos, pagination);
        }
        catch (Exception ex)
        {
            return ListPaymentsResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}