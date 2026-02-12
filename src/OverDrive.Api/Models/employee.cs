using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Employee
{
    public int Id { get; set; }
    public string Registration { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal CommissionRate { get; set; }
    public bool Active { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? FkAddress { get; set; }
    public int? FkBranch { get; set; }
    public int FkDepartment { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Address? FkAddressNavigation { get; set; }
    public virtual Branch? FkBranchNavigation { get; set; }
    public virtual Department FkDepartmentNavigation { get; set; } = null!;
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}