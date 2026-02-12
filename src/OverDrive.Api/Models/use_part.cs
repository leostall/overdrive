using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class UsePart
{
    public int FkPart { get; set; }
    public int FkServiceOrderItem { get; set; }
    public int Quantity { get; set; }
    public virtual Part FkPartNavigation { get; set; } = null!;
    public virtual ServiceOrderItem FkServiceOrderItemNavigation { get; set; } = null!;
}