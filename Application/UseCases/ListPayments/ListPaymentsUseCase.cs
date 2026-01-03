using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListPayments.DTO;
using Application.UseCases.ListPartners.DTO;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Application.UseCases.ListPayments;

public class ListPaymentsUseCase : IListPaymentsUseCase
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IVetorRepository _vetorRepository;

    public ListPaymentsUseCase(
        ICommissionRepository commissionRepository,
        IPartnerRepository partnerRepository,
        IBusinessRepository businessRepository,
        IVetorRepository vetorRepository)
    {
        _commissionRepository = commissionRepository;
        _partnerRepository = partnerRepository;
        _businessRepository = businessRepository;
        _vetorRepository = vetorRepository;
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

            // Converter strings de ordenação para enums
            var sortField = PaymentSortField.CreatedAt;
            var sortDirection = SortDirection.Descending;
            
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                PaymentSortFieldExtensions.TryParse(request.SortBy, out sortField);
            }
            
            if (!string.IsNullOrWhiteSpace(request.SortDirection))
            {
                SortDirectionExtensions.TryParse(request.SortDirection, out sortDirection);
            }

            // Buscar pagamentos com filtros
            var (payments, totalCount) = await _commissionRepository.GetPaymentsWithFiltersAsync(
                request.VetorId,
                request.PartnerId,
                request.StartDate,
                request.EndDate,
                request.Status,
                request.TipoPagamento,
                sortField,
                sortDirection,
                request.Page,
                request.PageSize,
                cancellationToken);

            // Converter para DTOs
            var paymentDtos = new List<PaymentListDto>();

            foreach (var payment in payments)
            {
                // Buscar a comissão primeiro
                var comission = await _commissionRepository.GetByIdAsync(payment.ComissionId, cancellationToken);
                if (comission == null)
                {
                    Console.WriteLine($"[ListPaymentsUseCase] ERRO: Comissão {payment.ComissionId} não encontrada para pagamento {payment.Id}");
                    continue;
                }
                
                // Buscar dados do parceiro
                var partner = await _partnerRepository.GetByIdAsync(payment.PartnerId, cancellationToken);
                
                // Buscar dados do negócio através da comissão
                var business = await _businessRepository.GetByIdAsync(comission.BussinessId, cancellationToken);

                // Buscar o parceiro do negócio para obter o VetorId
                Guid vetorId = Guid.Empty;
                string vetorName = "";
                
                if (business != null)
                {
                    var businessPartner = await _partnerRepository.GetByIdAsync(business.PartnerId, cancellationToken);
                    if (businessPartner != null)
                    {
                        vetorId = businessPartner.VetorId;
                        var vetor = await _vetorRepository.GetByIdAsync(vetorId, cancellationToken);
                        vetorName = vetor?.Name ?? "Vetor não encontrado";
                    }
                }

                var paymentDto = new PaymentListDto
                {
                    Id = payment.Id,
                    ComissionId = payment.ComissionId,
                    PartnerId = payment.PartnerId,
                    PartnerName = partner?.Name ?? "Parceiro não encontrado",
                    TipoPagamento = payment.TipoPagamento.ToLegacyString(),
                    Value = payment.Value,
                    Status = payment.Status.ToLegacyString(),
                    PaidOn = payment.PaidOn,
                    CreatedAt = comission.CreatedAt,
                    BusinessId = business?.Id ?? Guid.Empty,
                    BusinessDescription = business?.Observations ?? "",
                    BusinessTotalValue = business?.Value ?? 0,
                    BusinessDate = business?.CreatedAt ?? DateTime.MinValue,
                    VetorId = vetorId,
                    VetorName = vetorName
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