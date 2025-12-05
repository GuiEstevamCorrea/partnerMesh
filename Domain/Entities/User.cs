using Domain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public PermissionEnum Permission { get; private set; }
        public List<Vetor> Vetors { get; private set; } = new();

        public User() { }

        public User(string name, string email, string passwordHash, PermissionEnum permission, Guid? vetorId = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            ValidatePermission(permission);
            Permission = permission;
            if (vetorId.HasValue)
            {
                Vetor vetor = new Vetor();
                vetor.Id = vetorId.Value;
                Vetors.Add(vetor);
            }
        }

        private void ValidatePermission(PermissionEnum permission)
        {
            if (!Enum.IsDefined(typeof(PermissionEnum), permission))
            {
                throw new ArgumentException("Invalid permission value.");
            }
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void UpdatePermission(PermissionEnum permission)
        {
            ValidatePermission(permission);
            Permission = permission;
        }

        public void AssignVetor(Vetor vetor)
        {
            if (!Vetors.Any(v => v.Id == vetor.Id))
            {
                Vetors.Add(vetor);
            }
        }

        public void RemoveVetor(Guid vetorId)
        {
            var vetor = Vetors.FirstOrDefault(v => v.Id == vetorId);
            if (vetor != null)
            {
                Vetors.Remove(vetor);
            }
        }

        public void ClearVetors()
        {
            Vetors.Clear();
        }


    }
}
