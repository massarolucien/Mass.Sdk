using Mass.Sdk.Desktop.Models;

namespace Mass.Sdk.Mobile.Models;

public class MobileSession(MassClient client, string userId) : DesktopSession(client, userId);