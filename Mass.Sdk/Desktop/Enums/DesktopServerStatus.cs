namespace Mass.Sdk.Desktop.Enums;

public enum DesktopServerStatus
{
    None = -1,
    ServerOff = 0,
    ServerOn = 1,
    Uninitialized = 2,
    Opening = 3,
    Closing = 4,
    OutOfDate = 5,
    SaveCleaning = 6,
    Resetting = 7,
    Upgrading = 8,
    DiscOverflow = 9,
}