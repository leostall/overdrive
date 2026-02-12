using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class VehicleType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public byte? WheelCount { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}