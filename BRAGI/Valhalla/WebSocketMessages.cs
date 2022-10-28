using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BRAGI.Valhalla;

public class WebSocketMessage
{
    public string? UUID { get; set; }
    public string? Command { get; set; }
    public JsonObject? Params { get; set; }
}

/// <summary>
/// Codes that are returned when message is received.
/// </summary>
public enum RETURNCODE
{
    OK,
    UNKNOWN_COMMAND,
    INVALID_PARAMETER,
    COMMAND_ERROR,
    EXCEPTION
}

public class WebSocketResponse
{
    public string? UUID { get; private set; }
    public RETURNCODE Code { get; set; }
    public object Data { get; set; }
    public WebSocketResponse(string uuid, RETURNCODE code)
    {
        Code = code;
        Data = "";
        UUID = uuid;
    }
}
