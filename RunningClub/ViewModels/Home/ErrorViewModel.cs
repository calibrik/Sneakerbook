namespace RunningClub.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public required string Message { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
