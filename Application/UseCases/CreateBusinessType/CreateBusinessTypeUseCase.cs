using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.CreateBusinessType.DTO;
using Domain.Entities;
using Domain.ValueTypes;

namespace Application.UseCases.CreateBusinessType;

public class CreateBusinessTypeUseCase : ICreateBusinessTypeUseCase
{
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;

    public CreateBusinessTypeUseCase(
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository)
    {
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<CreateBusinessTypeResult> CreateAsync(CreateBusinessTypeRequest request, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar usuário atual
            var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
            if (currentUser == null || !currentUser.Active)
            {
                return CreateBusinessTypeResult.Failure("Usuário atual não encontrado ou inativo.");
            }

            // Verificar permissões - apenas AdminGlobal e AdminVetor podem criar tipos de negócio
            var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                               currentUser.Permission.HasFlag(PermissionEnum.AdminVetor);

            if (!hasPermission)
            {
                return CreateBusinessTypeResult.Failure("Usuário não tem permissão para criar tipos de negócio.");
            }

            // Validar se o nome já existe
            var nameExists = await _businessTypeRepository.NameExistsAsync(request.Name, cancellationToken);
            if (nameExists)
            {
                return CreateBusinessTypeResult.Failure("Já existe um tipo de negócio com este nome.");
            }

            // Criar novo tipo de negócio
            var businessType = new BusinessType(
                request.Name.Trim(),
                request.Description.Trim(),
                currentUserId
            );

            // Salvar no repositório
            await _businessTypeRepository.AddAsync(businessType, cancellationToken);

            // Retornar resultado de sucesso
            var businessTypeDto = BusinessTypeCreateDto.FromEntity(businessType);
            return CreateBusinessTypeResult.Success(businessTypeDto);
        }
        catch (Exception ex)
        {
            return CreateBusinessTypeResult.Failure($"Erro interno: {ex.Message}");
        }
    }
}