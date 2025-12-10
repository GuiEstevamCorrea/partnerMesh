using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.ListPartners.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.ListPartners;

public class ListPartnersUseCase : IListPartnersUseCase
{
    private readonly IPartnerRepository _partnerRepository;
    private readonly IVetorRepository _vetorRepository;
    private readonly IUserRepository _userRepository;

    public ListPartnersUseCase(
        IPartnerRepository partnerRepository, 
        IVetorRepository vetorRepository,
        IUserRepository userRepository)
    {
        _partnerRepository = partnerRepository;
        _vetorRepository = vetorRepository;
        _userRepository = userRepository;
    }

    public async Task<ListPartnersResult> ListAsync(ListPartnersRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
            if (currentUser == null || !currentUser.Active)
            {
                return ListPartnersResult.Failure("Usuário atual não encontrado ou inativo.");
            }

            // Verificar permissões
            var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                               currentUser.Permission.HasFlag(PermissionEnum.AdminVetor) ||
                               currentUser.Permission.HasFlag(PermissionEnum.Operador);

            if (!hasPermission)
            {
                return ListPartnersResult.Failure("Usuário não tem permissão para listar parceiros.");
            }

            // Se for AdminVetor ou Operador, filtrar apenas parceiros do seu vetor
            ListPartnersRequest filteredRequest = request;
            if (!currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal))
            {
                // Para AdminVetor e Operador, precisamos buscar o vetor associado ao usuário
                var userVetores = currentUser.UserVetores;
                if (userVetores.Any())
                {
                    var vetorId = userVetores.First().VetorId;
                    filteredRequest = request with { VetorId = vetorId };
                }
                else
                {
                    return ListPartnersResult.Failure("Usuário não está associado a nenhum vetor.");
                }
            }

            // Validar paginação
            if (filteredRequest.Page < 1)
            {
                filteredRequest = filteredRequest with { Page = 1 };
            }

            if (filteredRequest.PageSize < 1 || filteredRequest.PageSize > 100)
            {
                filteredRequest = filteredRequest with { PageSize = 20 };
            }

            // Buscar parceiros com filtros
            var (partners, totalCount) = await _partnerRepository.GetFilteredAsync(filteredRequest, cancellationToken);

            // Buscar informações adicionais (vetores e recomendadores)
            var partnerDtos = new List<PartnerDto>();
            
            foreach (var partner in partners)
            {
                // Buscar nome do vetor
                var vetor = await _vetorRepository.GetByIdAsync(partner.VetorId, cancellationToken);
                var vetorName = vetor?.Name ?? "Vetor não encontrado";

                // Buscar nome do recomendador (se houver)
                string? recommenderName = null;
                if (partner.RecommenderId.HasValue)
                {
                    var recommender = await _partnerRepository.GetByIdAsync(partner.RecommenderId.Value, cancellationToken);
                    recommenderName = recommender?.Name;
                }

                partnerDtos.Add(PartnerDto.FromEntity(partner, vetorName, recommenderName));
            }

            // Criar informação de paginação
            var totalPages = (int)Math.Ceiling((double)totalCount / filteredRequest.PageSize);
            var pagination = new PaginationInfo
            {
                Page = filteredRequest.Page,
                PageSize = filteredRequest.PageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };

            return ListPartnersResult.Success(partnerDtos, pagination);
        }
        catch (Exception ex)
        {
            return ListPartnersResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}