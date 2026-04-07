namespace Mass.Sdk.Models;

public record Progress(
    int Step,
    int Total,
    int Percentage,
    string Message
);