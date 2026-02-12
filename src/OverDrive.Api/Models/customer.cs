using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Customer
{
    public int Id { get; set; }
    public string CpfCnpj { get; set; } = null!;
    public string CustomerType { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool Active { get; set; }
    public int? FkAddress { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Address? FkAddressNavigation { get; set; }
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}