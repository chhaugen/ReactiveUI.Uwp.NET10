namespace ReactiveUIApp.Core.Exceptions;

public class ServiceMissingException<T> : Exception
{
    public ServiceMissingException() : base(GenerateMessage())
    {
    }

    public ServiceMissingException(Exception? innerException) : base(GenerateMessage(), innerException)
    {
    }

    internal static string GenerateMessage()
        => $"Unable to find service {typeof(T).Name}.";
}
