using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class ServiceOrderItem
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
    public int FkServiceOrder { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual ServiceOrder FkServiceOrderNavigation { get; set; } = null!;
    public virtual ICollection<UsePart> UseParts { get; set; } = new List<UsePart>();
}