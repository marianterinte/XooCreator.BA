using System;

namespace XooCreator.BA.Common.Exceptions;

/// <summary>
/// Exception thrown when a concurrency conflict occurs during entity update
/// </summary>
public class ConcurrencyException : Exception
{
    public string EntityName { get; }
    public string EntityId { get; }

    public ConcurrencyException(string entityName, string entityId)
        : base($"The {entityName} with ID '{entityId}' was modified by another user. Please reload and try again.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public ConcurrencyException(string entityName, string entityId, string message)
        : base(message)
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    public ConcurrencyException(string entityName, string entityId, string message, Exception innerException)
        : base(message, innerException)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}