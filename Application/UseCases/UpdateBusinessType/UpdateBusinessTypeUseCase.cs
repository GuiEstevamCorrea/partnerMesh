using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.UseCases.UpdateBusinessType.DTO;
using Domain.ValueTypes;

namespace Application.UseCases.UpdateBusinessType;

public class UpdateBusinessTypeUseCase : IUpdateBusinessTypeUseCase
{
    private readonly IBusinessTypeRepository _businessTypeRepository;
    private readonly IUserRepository _userRepository;

    public UpdateBusinessTypeUseCase(
        IBusinessTypeRepository businessTypeRepository,
        IUserRepository userRepository)
    {
        _businessTypeRepository = businessTypeRepository;
        _userRepository = userRepository;
    }

    public async Task<UpdateBusinessTypeResult> ExecuteAsync(
        Guid businessTypeId, 
        UpdateBusinessTypeRequest request, 
        Guid userId)
    {
        // Validar usuário atual
        var currentUser = await _userRepository.GetByIdAsync(userId);
        if (currentUser == null || !currentUser.Active)
        {
            throw new UnauthorizedAccessException("Usuário não encontrado ou inativo.");
        }

        // Verificar permissões - apenas AdminGlobal e AdminVetor podem atualizar tipos de negócio
        var hasPermission = currentUser.Permission.HasFlag(PermissionEnum.AdminGlobal) ||
                           currentUser.Permission.HasFlag(PermissionEnum.AdminVetor);

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("Usuário não tem permissão para atualizar tipos de negócio.");
        }

        // Buscar tipo de negócio existente
        var businessType = await _businessTypeRepository.GetByIdAsync(businessTypeId);
        if (businessType == null)
        {
            throw new ArgumentException("Tipo de negócio não encontrado.");
        }

        // Validar campos obrigatórios
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("O nome é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException("A descrição é obrigatória.");
        }

        // Validar se o novo nome já existe (excluindo o atual)
        var nameExists = await _businessTypeRepository.NameExistsAsync(request.Name.Trim(), businessTypeId);
        if (nameExists)
        {
            throw new ArgumentException("Já existe outro tipo de negócio com este nome.");
        }

        // Atualizar informações
        businessType.UpdateInfo(request.Name.Trim(), request.Description.Trim(), userId);

        // Salvar alterações
        await _businessTypeRepository.UpdateAsync(businessType);

        // Retornar resultado
        return UpdateBusinessTypeResult.From(businessType);
    }
}