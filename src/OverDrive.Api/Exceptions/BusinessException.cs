namespace OverDrive.Api.Exceptions;

public class BusinessException : Exception
{
    public ErrorCode Code { get; }

    public BusinessException(ErrorCode code) : base(ErrorMessages.GetMessage(code))
    {
        Code = code;
    }

    public BusinessException(ErrorCode code, string message) : base(message)
    {
        Code = code;
    }

    public BusinessException(string message) : base(message)
    {
        Code = ErrorCode.Unknown;
    }
}