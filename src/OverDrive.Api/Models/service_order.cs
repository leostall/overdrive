using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class ServiceOrder
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public DateTime OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalValue { get; set; }
    public string? Notes { get; set; }
    public int FkVehicle { get; set; }
    public int? FkBranch { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Vehicle FkVehicleNavigation { get; set; } = null!;
    public virtual Branch? FkBranchNavigation { get; set; }
    public virtual ICollection<ServiceOrderItem> ServiceOrderItems { get; set; } = new List<ServiceOrderItem>();
}