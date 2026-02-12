using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Vehicle
{
    public int Id { get; set; }
    public string Chassi { get; set; } = null!;
    public string? Plate { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public int? Mileage { get; set; }
    public string? Condition { get; set; }
    public string? Status { get; set; }
    public decimal Value { get; set; }
    public int FkAddress { get; set; }
    public int FkCustomer { get; set; }
    public int? FkVehicleType { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Address FkAddressNavigation { get; set; } = null!;
    public virtual Customer FkCustomerNavigation { get; set; } = null!;
    public virtual VehicleType? FkVehicleTypeNavigation { get; set; }
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
}