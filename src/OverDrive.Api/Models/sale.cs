using System;
using System.Collections.Generic;

namespace OverDrive.Api.Models;

public partial class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal AdditionalFee { get; set; }
    public decimal Total { get; set; }
    public int FkCustomer { get; set; }
    public int? FkBranch { get; set; }
    public int? FkEmployee { get; set; }
    public int? FkPayment { get; set; }
    public int? FkStatus { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public virtual Branch? FkBranchNavigation { get; set; }
    public virtual Customer FkCustomerNavigation { get; set; } = null!;
    public virtual Employee? FkEmployeeNavigation { get; set; }
    public virtual Payment? FkPaymentNavigation { get; set; }
    public virtual Status? FkStatusNavigation { get; set; }
    public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}