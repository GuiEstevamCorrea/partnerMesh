using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueTypes;
using Domain.Extensions;
using Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class CommissionRepository : ICommissionRepository
{
    private readonly PartnerMeshDbContext _context;

    public CommissionRepository(PartnerMeshDbContext context)
    {
        _context = context;
        
        // Fix one-time: Preencher PaidOn em pagamentos antigos
        _ = Task.Run(async () =>
        {
            try
            {
                var sql = @"UPDATE ComissionPayments 
                           SET PaidOn = (SELECT c.CreatedAt FROM Comissions c WHERE c.Id = ComissionPayments.ComissionId)
                           WHERE Status = 1 AND PaidOn IS NULL";
                
                var count = await _context.Database.ExecuteSqlRawAsync(sql);
                if (count > 0)
                {
                    Console.WriteLine($"[CommissionRepository] {count} pagamento(s) antigo(s) atualizados com PaidOn.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommissionRepository] Erro ao atualizar PaidOn: {ex.Message}");
            }
        });
    }

    public async Task<Comission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Comissions
            .Include(c => c.Bussiness)
                .ThenInclude(b => b.Partner)
            .Include(c => c.Pagamentos)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Comission?> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        return await _context.Comissions
            .Include(c => c.Bussiness)
                .ThenInclude(b => b.Partner)
            .Include(c => c.Pagamentos)
            .FirstOrDefaultAsync(c => c.BussinessId == businessId, cancellationToken);
    }

    public async Task<IEnumerable<Comission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Comissions
            .Include(c => c.Bussiness)
                .ThenInclude(b => b.Partner)
            .Include(c => c.Pagamentos)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        await _context.Comissions.AddAsync(commission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        _context.Comissions.Update(commission);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Comission commission, CancellationToken cancellationToken = default)
    {
        _context.Comissions.Remove(commission);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IEnumerable<ComissionPayment> payments, int totalCount)> GetPaymentsWithFiltersAsync(
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
        var allPayments = _context.Comissions
            .Include(c => c.Bussiness)
                .ThenInclude(b => b.Partner)
            .SelectMany(c => c.Pagamentos)
            .AsQueryable();

        Console.WriteLine($"[CommissionRepository] Filtros: status={status}, startDate={startDate?.ToString("yyyy-MM-dd")}, endDate={endDate?.ToString("yyyy-MM-dd")}");

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
                Console.WriteLine($"[CommissionRepository] Aplicado filtro de status: {statusEnum}");
            }
        }

        if (!string.IsNullOrEmpty(tipoPagamento))
        {
            if (PaymentTypeExtensions.TryParse(tipoPagamento, out var typeEnum))
            {
                allPayments = allPayments.Where(p => p.TipoPagamento == typeEnum);
            }
        }

        // Se o status for "Pago", filtrar por PaidOn ao invés de CreatedAt
        var isPaidStatus = !string.IsNullOrEmpty(status) && 
                          PaymentStatusExtensions.TryParse(status, out var paidStatusCheck) && 
                          paidStatusCheck == PaymentStatus.Pago;

        if (startDate.HasValue)
        {
            if (isPaidStatus)
            {
                // Para pagamentos pagos, filtrar pela data de pagamento
                Console.WriteLine($"[CommissionRepository] Aplicando filtro PaidOn >= {startDate.Value.Date:yyyy-MM-dd}");
                allPayments = allPayments.Where(p => p.PaidOn.HasValue && p.PaidOn.Value.Date >= startDate.Value.Date);
            }
            else
            {
                // Para outros casos, filtrar pela data de criação da comissão
                Console.WriteLine($"[CommissionRepository] Aplicando filtro CreatedAt >= {startDate.Value.Date:yyyy-MM-dd}");
                allPayments = allPayments.Where(p => p.Comission.CreatedAt >= startDate.Value);
            }
        }

        if (endDate.HasValue)
        {
            if (isPaidStatus)
            {
                // Para pagamentos pagos, filtrar pela data de pagamento
                allPayments = allPayments.Where(p => p.PaidOn.HasValue && p.PaidOn.Value.Date <= endDate.Value.Date);
            }
            else
            {
                // Para outros casos, filtrar pela data de criação da comissão
                allPayments = allPayments.Where(p => p.Comission.CreatedAt <= endDate.Value);
            }
        }

        // Filtro por vetorId usando a relação Business -> Partner -> Vetor
        if (vetorId.HasValue)
        {
            allPayments = allPayments.Where(p => 
                p.Comission.Bussiness != null && 
                p.Comission.Bussiness.Partner != null && 
                p.Comission.Bussiness.Partner.VetorId == vetorId.Value);
        }

        // Contar total antes da paginação
        var totalCount = await allPayments.CountAsync(cancellationToken);

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
        var pagedPayments = await allPayments
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (pagedPayments.AsEnumerable(), totalCount);
    }
}