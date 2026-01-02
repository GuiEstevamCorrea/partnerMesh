using System;
using System.Collections.Generic;

namespace Api.Models;

public partial class Partner
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid VetorId { get; set; }

    public Guid? RecommenderId { get; set; }

    public virtual ICollection<Business> Businesses { get; set; } = new List<Business>();

    public virtual ICollection<Partner> InverseRecommender { get; set; } = new List<Partner>();

    public virtual Partner? Recommender { get; set; }

    public virtual Vetore Vetor { get; set; } = null!;
}
