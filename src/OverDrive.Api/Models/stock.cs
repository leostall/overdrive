using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Stock
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Note { get; set; }
    public int? FkBranch { get; set; }
    public int FkAddress { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Address FkAddressNavigation { get; set; } = null!;
    public virtual Branch? FkBranchNavigation { get; set; }
    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}