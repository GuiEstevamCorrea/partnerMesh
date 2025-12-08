using System;
using System.Collections.Generic;
using Domain.ValueObjects;
using Domain.ValueTypes;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public PermissionEnum Permission { get; private set; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<UserVetor> UserVetores => _userVetores.AsReadOnly();
    private readonly List<UserVetor> _userVetores = new();

    protected User() { }

    public User(string name, string email, string passwordHash, PermissionEnum permission)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Permission = permission;
        Active = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddVetor(Guid vetorId)
    {
        _userVetores.Add(new UserVetor(Id, vetorId));
    }

    public bool PasswordMatches(string plain)
       => BCrypt.Net.BCrypt.Verify(plain, PasswordHash);

    public bool HasActiveVetor()
        => UserVetores == null || UserVetores.Count == 0 || UserVetores.Any(v => v.Active);
}
