namespace Application.UseCases.UpdateVetor.DTO;

public sealed record UpdateVetorRequest(
    string? Name = null,
    string? Email = null,
    bool? Active = null);