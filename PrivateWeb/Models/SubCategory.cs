using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class SubCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int Status { get; set; }

    public string? Image { get; set; }

    public long CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int IsFeatured { get; set; }

    public string ShowHome { get; set; } = null!;
}
