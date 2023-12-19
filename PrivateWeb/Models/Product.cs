using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Keywords { get; set; }

    public string? Description { get; set; }

    public string? Detail { get; set; }

    public string? Care { get; set; }

    public int Price { get; set; }

    public int Amount { get; set; }

    public int Status { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int? PromotionId { get; set; }

    public int? ImagesId { get; set; }

    public string? ShowHome { get; set; }

    public int IsFeatured { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

	public virtual Category? Category { get; set; }

	public virtual ProductsImage? Images { get; set; }

	public virtual Promotion? Promotion { get; set; }

	public virtual SubCategory? SubCategory { get; set; }

	public virtual ICollection<Variantss> Variantsses { get; set; } = new List<Variantss>();
}
