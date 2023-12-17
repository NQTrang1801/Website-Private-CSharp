using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public decimal Value { get; set; }

    public string Code { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Variantss> Variantsses { get; set; } = new List<Variantss>();
}
