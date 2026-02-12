using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Branch
{
    public int Id { get; set; }
    public string CorporateName { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool Active { get; set; }
    public int FkAddress { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Address FkAddressNavigation { get; set; } = null!;
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}