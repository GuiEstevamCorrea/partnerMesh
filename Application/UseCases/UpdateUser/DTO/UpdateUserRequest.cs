using Domain.ValueTypes;

namespace Application.UseCases.UpdateUser.DTO;

public sealed record UpdateUserRequest(
    string? Name = null,
    string? Email = null,
    PermissionEnum? Permission = null,
    Guid? VetorId = null,
    string? Password = null);