using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class Variantss
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public int ColorId { get; set; }

    public int? PromotionId { get; set; }

    public string? Image { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public int Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Color Color { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Promotion? Promotion { get; set; }

    public virtual Size Size { get; set; } = null!;
}
