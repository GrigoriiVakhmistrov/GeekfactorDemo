namespace TestApp.Application.Services;

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message)
    {
    }

    public ServiceUnavailableException()
    {
    }

    public ServiceUnavailableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}