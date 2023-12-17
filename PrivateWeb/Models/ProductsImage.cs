using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class ProductsImage
{
    public int Id { get; set; }

    public string Image1 { get; set; } = null!;

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public string? Image4 { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
