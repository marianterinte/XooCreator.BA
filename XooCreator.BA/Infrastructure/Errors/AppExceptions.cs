using System.Diagnostics.CodeAnalysis;

namespace XooCreator.BA.Infrastructure.Errors;

public abstract class AppException : Exception
{
    public string Layer { get; }
    public string Operation { get; }

    protected AppException(string layer, string operation, string message, Exception? inner = null)
        : base(message, inner)
    {
        Layer = layer;
        Operation = operation;
    }
}

public sealed class RepositoryException : AppException
{
    public RepositoryException(string operation, string message, Exception? inner = null)
        : base("Repository", operation, message, inner) { }
}

public sealed class ServiceException : AppException
{
    public ServiceException(string operation, string message, Exception? inner = null)
        : base("Service", operation, message, inner) { }
}

public sealed class EndpointException : AppException
{
    public EndpointException(string operation, string message, Exception? inner = null)
        : base("Endpoint", operation, message, inner) { }
}

public sealed class ValidationException : AppException
{
    public ValidationException(string operation, string message, Exception? inner = null)
        : base("Service", operation, message, inner) { }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string operation, string message, Exception? inner = null)
        : base("Service", operation, message, inner) { }
}
