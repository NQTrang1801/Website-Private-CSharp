using System;
using System.Collections.Generic;

namespace PrivateWeb.Models;

public partial class TempImage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
