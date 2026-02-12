namespace OverDrive.Api.Dtos.Employees;

public class EmployeeResponse
{
    public int EmployeeId { get; set; }
    public string Registration { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal CommissionRate { get; set; }
    public bool Active { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? AddressId { get; set; }
    public int? BranchId { get; set; }
    public int DepartmentId { get; set; }
}
