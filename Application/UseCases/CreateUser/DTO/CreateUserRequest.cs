using Domain.ValueTypes;

namespace Application.UseCases.CreateUser.DTO;

public sealed record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    PermissionEnum Permission,
    Guid? VetorId = null);