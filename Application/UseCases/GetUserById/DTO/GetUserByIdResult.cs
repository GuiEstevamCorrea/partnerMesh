namespace Application.UseCases.GetUserById.DTO;

public sealed record GetUserByIdResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public UserDetailInfo? User { get; init; }

    private GetUserByIdResult(bool isSuccess, string message, UserDetailInfo? user = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        User = user;
    }

    public static GetUserByIdResult Success(UserDetailInfo user)
        => new(true, "Usuário encontrado com sucesso.", user);

    public static GetUserByIdResult Failure(string message)
        => new(false, message);

    public static GetUserByIdResult NotFound()
        => new(false, "Usuário não encontrado.");
}

public sealed record UserDetailInfo(
    Guid Id,
    string Name,
    string Email,
    string Permission,
    bool Active,
    DateTime CreatedAt,
    IEnumerable<UserVetorDetail> Vetores)
{
    public bool IsAdminGlobal => Permission == "AdminGlobal";
    public bool IsAdminVetor => Permission == "AdminVetor";
    public bool IsOperador => Permission == "Operador";
}

public sealed record UserVetorDetail(
    Guid VetorId,
    string VetorName,
    string VetorEmail,
    bool VetorActive,
    DateTime AssociatedAt,
    bool AssociationActive);