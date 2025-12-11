namespace Application.UseCases.GetBusinessTypeById.DTO;

public sealed record GetBusinessTypeByIdRequest
{
    /// <summary>
    /// ID do tipo de negócio a ser buscado
    /// </summary>
    public Guid BusinessTypeId { get; init; }
}