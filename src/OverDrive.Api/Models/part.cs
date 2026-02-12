using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Part
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int FkStock { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Stock FkStockNavigation { get; set; } = null!;
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public virtual ICollection<UsePart> UseParts { get; set; } = new List<UsePart>();
}