using Mass.Sdk.Helpers;

namespace Mass.Sdk.Models;

public class ApiResponse
{
    public int Code { get; set; }
    public string Msg { get; set; } = string.Empty;
    public override string ToString() => JsonHelper.SerializeSnakeCase(this);
}
public class ApiResponse<TData> : ApiResponse
{
    public TData? Data { get; set; }
}