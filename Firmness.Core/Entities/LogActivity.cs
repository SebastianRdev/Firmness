namespace Firmness.Core.Entities;

using Firmness.Core.Entities;

public class LogActivity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ApplicationUser User { get; set; } = null!;
}