using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class SaleItem
{
    public int Id { get; set; }
    public string ItemType { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int? FkPart { get; set; }
    public int? FkVehicle { get; set; }
    public int FkSale { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Part? FkPartNavigation { get; set; }
    public virtual Vehicle? FkVehicleNavigation { get; set; }
    public virtual Sale FkSaleNavigation { get; set; } = null!;
}