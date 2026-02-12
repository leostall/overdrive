using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Payment
{
    public int Id { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}