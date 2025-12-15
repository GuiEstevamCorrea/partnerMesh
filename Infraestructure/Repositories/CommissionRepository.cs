using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;

namespace Infraestructure.Repositories;

public class CommissionRepository : ICommissionRepository
{
    private readonly List<Comission> _commissions = new();

    public Task<Comission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var commission = _commissions.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(commission);
    }

    public Task<Comission?> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        var commission = _commissions.FirstOrDefault(c => c.BussinessId == businessId);
        return Task.FromResult(commission);
    }

    public Task<IEnumerable<Comission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Comission>>(_commissions.ToList());
    }

    public Task AddAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        _commissions.Add(commission);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        var existing = _commissions.FirstOrDefault(c => c.Id == commission.Id);
        if (existing != null)
        {
            _commissions.Remove(existing);
            _commissions.Add(commission);
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        _commissions.Remove(commission);
        return Task.CompletedTask;
    }

    public Task<(IEnumerable<ComissionPayment> payments, int totalCount)> GetPaymentsWithFiltersAsync(
        Guid? vetorId = null,
        Guid? partnerId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        string? tipoPagamento = null,
        PaymentSortField sortBy = PaymentSortField.CreatedAt,
        SortDirection sortDirection = SortDirection.Descending,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // Obter todos os pagamentos de todas as comissões
        var allPayments = _commissions.SelectMany(c => c.Pagamentos).AsQueryable();

        // Aplicar filtros
        if (partnerId.HasValue)
        {
            allPayments = allPayments.Where(p => p.PartnerId == partnerId);
        }

        if (!string.IsNullOrEmpty(status))
        {
            if (PaymentStatusExtensions.TryParse(status, out var statusEnum))
            {
                allPayments = allPayments.Where(p => p.Status == statusEnum);
            }
        }

        if (!string.IsNullOrEmpty(tipoPagamento))
        {
            if (PaymentTypeExtensions.TryParse(tipoPagamento, out var typeEnum))
            {
                allPayments = allPayments.Where(p => p.TipoPagamento == typeEnum);
            }
        }

        if (startDate.HasValue)
        {
            allPayments = allPayments.Where(p => p.Comission.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            allPayments = allPayments.Where(p => p.Comission.CreatedAt <= endDate.Value);
        }

        // Filtro por vetorId usando a relação Business -> Partner -> Vetor
        if (vetorId.HasValue)
        {
            var commissionsForVetor = _commissions
                .Where(c => c.Bussiness != null && 
                           c.Bussiness.Partner != null && 
                           c.Bussiness.Partner.VetorId == vetorId.Value)
                .Select(c => c.Id)
                .ToHashSet();
            
            allPayments = allPayments.Where(p => commissionsForVetor.Contains(p.ComissionId));
        }

        // Contar total antes da paginação
        var totalCount = allPayments.Count();

        // Aplicar ordenação
        var isAscending = sortDirection == SortDirection.Ascending;
        
        allPayments = sortBy switch
        {
            PaymentSortField.Value => isAscending
                ? allPayments.OrderBy(p => p.Value)
                : allPayments.OrderByDescending(p => p.Value),
            PaymentSortField.Status => isAscending
                ? allPayments.OrderBy(p => p.Status)
                : allPayments.OrderByDescending(p => p.Status),
            PaymentSortField.PaidOn => isAscending
                ? allPayments.OrderBy(p => p.PaidOn ?? DateTime.MinValue)
                : allPayments.OrderByDescending(p => p.PaidOn ?? DateTime.MinValue),
            _ => isAscending
                ? allPayments.OrderBy(p => p.Comission.CreatedAt)
                : allPayments.OrderByDescending(p => p.Comission.CreatedAt)
        };

        // Aplicar paginação
        var pagedPayments = allPayments
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((pagedPayments.AsEnumerable(), totalCount));
    }
}