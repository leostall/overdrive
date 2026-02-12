namespace OverDrive.Api.Dtos.Employees;

public class UpdateEmployeeRequest
{
    public string? Registration { get; set; }
    public string? Name { get; set; }
    public decimal? CommissionRate { get; set; }
    public bool? Active { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? AddressId { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
}
