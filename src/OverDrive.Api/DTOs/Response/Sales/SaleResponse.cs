using System;
using System.Collections.Generic;

namespace OverDrive.Api.Dtos.Sales;

public class SaleResponse
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal AdditionalFee { get; set; }
    public decimal Total { get; set; }
    public int CustomerId { get; set; }
    public int? BranchId { get; set; }
    public int? EmployeeId { get; set; }
    public int? PaymentId { get; set; }
    public int? StatusId { get; set; }
    public List<SaleItemResponse> Items { get; set; } = new();
}
