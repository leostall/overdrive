using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}