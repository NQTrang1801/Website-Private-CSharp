using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class Size
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Variantss> Variantsses { get; set; } = new List<Variantss>();
}
