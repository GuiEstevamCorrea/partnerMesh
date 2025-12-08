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

    public void RemoveVetor(Guid vetorId)
    {
        var userVetor = _userVetores.FirstOrDefault(uv => uv.VetorId == vetorId);
        if (userVetor != null)
        {
            userVetor.Deactivate();
        }
    }

    public void ClearVetores()
    {
        foreach (var userVetor in _userVetores)
        {
            userVetor.Deactivate();
        }
    }

    public void UpdateVetor(Guid? vetorId)
    {
        // Remove todos os vetores ativos
        ClearVetores();
        
        // Adiciona o novo vetor se fornecido
        if (vetorId.HasValue)
        {
            AddVetor(vetorId.Value);
        }
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail;
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void UpdatePermission(PermissionEnum newPermission)
    {
        Permission = newPermission;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }

    public bool PasswordMatches(string plain)
       => BCrypt.Net.BCrypt.Verify(plain, PasswordHash);

    public bool HasActiveVetor()
        => UserVetores == null || UserVetores.Count == 0 || UserVetores.Any(v => v.Active);
}
