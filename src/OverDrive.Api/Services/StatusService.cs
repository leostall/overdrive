using OverDrive.Api.Dtos.Statuses;
using OverDrive.Api.Exceptions;
using OverDrive.Api.Models;
using OverDrive.Api.Repositories.Interfaces;
using OverDrive.Api.Services.Interfaces;

namespace OverDrive.Api.Services;

public class StatusService : IStatusService
{
    private readonly IStatusRepository _repository;

    public StatusService(IStatusRepository repository)
    {
        _repository = repository;
    }

    public StatusResponse Create(CreateStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Description is required.");

        string strDescription = request.Description.Trim();

        Status? objExists = _repository.GetByDescription(strDescription);
        if (objExists != null)
            throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Status already registered.");

        Status objStatus = new()
        {
            Description = strDescription
        };

        _repository.Add(objStatus);
        _repository.SaveChanges();

        return MapToResponse(objStatus);
    }

    public StatusResponse Update(int statusId, UpdateStatusRequest request)
    {
        Status objStatus = _repository.GetById(statusId)
            ?? throw new NotFoundException($"Status not found. Id={statusId}");

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            string strDescription = request.Description.Trim();
            Status? objExists = _repository.GetByDescription(strDescription);
            if (objExists != null && objExists.Id != objStatus.Id)
                throw new BusinessException(ErrorCodes.Sale.MissingRequiredFields, "Status already registered.");

            objStatus.Description = strDescription;
        }

        _repository.SaveChanges();

        return MapToResponse(objStatus);
    }

    public void Delete(int statusId)
    {
        Status objStatus = _repository.GetById(statusId)
            ?? throw new NotFoundException($"Status not found. Id={statusId}");

        if (_repository.HasSales(statusId))
            throw new BusinessException(ErrorCodes.Sale.Blocked, "Status cannot be deleted because it is linked to sales.");

        _repository.Remove(objStatus);
        _repository.SaveChanges();
    }

    public StatusResponse GetById(int statusId)
    {
        Status objStatus = _repository.GetById(statusId)
            ?? throw new NotFoundException($"Status not found. Id={statusId}");

        return MapToResponse(objStatus);
    }

    public List<StatusResponse> GetAll()
    {
        return _repository.GetAll().Select(MapToResponse).ToList();
    }

    private static StatusResponse MapToResponse(Status status)
    {
        return new StatusResponse
        {
            StatusId = status.Id,
            Description = status.Description
        };
    }
}
