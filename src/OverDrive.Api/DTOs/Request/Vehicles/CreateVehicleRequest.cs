namespace OverDrive.Api.Dtos.Vehicles;

public class CreateVehicleRequest
{
    public string Chassi { get; set; } = string.Empty;
    public string? Plate { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int? Mileage { get; set; }
    public string? Condition { get; set; }
    public string? Status { get; set; }
    public decimal Value { get; set; }
    public int AddressId { get; set; }
    public int CustomerId { get; set; }
    public int? VehicleTypeId { get; set; }
}
