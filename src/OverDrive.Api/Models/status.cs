using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Status
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}