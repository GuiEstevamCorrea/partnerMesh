namespace Application.UseCases.GetVetorById.DTO;

public sealed record GetVetorByIdRequest(
    Guid VetorId)
{
    public static GetVetorByIdRequest Create(Guid vetorId) => new(vetorId);
}