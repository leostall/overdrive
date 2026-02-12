namespace OverDrive.Api.Dtos.Branches;

public class BranchReportResponse
{
    public int BranchId { get; set; }
    public string CorporateName { get; set; } = string.Empty;
    public int EmployeesCount { get; set; }
    public int SalesCount { get; set; }
    public int OpenServiceOrdersCount { get; set; }
    public int StockPartsQuantity { get; set; }
    public decimal StockTotalValue { get; set; }
}
