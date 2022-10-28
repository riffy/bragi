using BRAGI.Util;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum LeaveRoomError
{
    BRAGI_INITIALIZATION_ERROR
}
public class LeaveRoomParameter : BragiParameter
{
    public string RoomName { get; private set; }
    public LeaveRoomParameter(string roomName)
    {
        RoomName = roomName;
    }
}

public class LeaveRoom : BragiCommand<LeaveRoomParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        return JsonValidator.CheckParameters<LeaveRoomParameter>(parameters);
    }

    public async override Task<object> ExecuteInternal(LeaveRoomParameter? parameters)
    {
        if (Bragi.Instance!.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)LeaveRoomError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
        return await BragiRoom.BragiRoom.LeaveRoom(parameters!.RoomName);
    }

    public override LeaveRoomParameter? ParseParameters(JsonObject? parameters)
    {
        if (!CheckParameters(parameters)) return null;
        return new LeaveRoomParameter(
            (string)parameters!["RoomName"]!);
    }
}