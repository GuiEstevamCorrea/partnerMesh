namespace Application.UseCases.CreateVetor.DTO;

public sealed record CreateVetorRequest(
    string Name,
    string Email);