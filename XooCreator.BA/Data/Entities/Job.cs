namespace XooCreator.BA.Data;

public enum JobType { Image, Story }
public enum JobStatus { Queued, Running, Done, Error }

public class Job
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserAlchimalia User { get; set; } = null!;
    public JobType Type { get; set; }
    public string PayloadJson { get; set; } = "{}";
    public JobStatus Status { get; set; } = JobStatus.Queued;
    public string? ResultUrl { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
