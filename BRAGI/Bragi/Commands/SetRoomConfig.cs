using BRAGI.Bragi.BragiRoom;
using BRAGI.Util;
using SharpHook.Native;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum SetRoomConfigError
{
    BRAGI_INITIALIZATION_ERROR,
    UNKNOWN_ROOM
}

public class SetRoomConfigParameter : BragiParameter
{
    [OptionalParameter] // Is marked optional as this may be omitted when performed during JoinRoom
    public string RoomName { get; private set; }
    [OptionalParameter]
    public bool ForceP2T { get; private set; }
    [OptionalParameter]
    public KeyCode P2TKey { get; private set; }
    public SetRoomConfigParameter(string roomName, bool forceP2T, KeyCode p2TKey)
    {
        RoomName = roomName;
        ForceP2T = forceP2T;
        P2TKey = p2TKey;
    }

    public static SetRoomConfigParameter GetDefault(string roomName)
    {
        return new SetRoomConfigParameter(
            roomName,
            false,
            KeyCode.CharUndefined
            );
    }
}

public class SetRoomConfig : BragiCommand<SetRoomConfigParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        return JsonValidator.CheckParameters<SetRoomConfigParameter>(parameters);
    }

    public override async Task<object> ExecuteInternal(SetRoomConfigParameter? param)
    {
        if (Bragi.Instance!.State != BRAGISTATE.INITIALIZED)
            throw new CommandException((int)SetRoomConfigError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
        if (!BragiRoom.BragiRoom.Collection.ContainsKey(param!.RoomName))
            throw new CommandException((int)SetRoomConfigError.UNKNOWN_ROOM, "Room unknown");
        if (BragiRoom.BragiRoom.Collection.TryGetValue(param!.RoomName, out BragiRoom.BragiRoom? room))
        {
            room.ApplyConfig(param);
        }
        else
        {
            throw new CommandException((int)SetRoomConfigError.UNKNOWN_ROOM, "Room unknown");
        }
        return "";
    }

    public override SetRoomConfigParameter? ParseParameters(JsonObject? parameters)
    {
        if (!CheckParameters(parameters)) return null;
        return new SetRoomConfigParameter(
            (string)parameters!["RoomName"]!,
            parameters.ContainsKey("ForceP2T") && (bool)parameters!["ForceP2T"]!,
            parameters.ContainsKey("P2TKey") ? (KeyCode)(int)parameters!["P2TKey"]! : KeyCode.CharUndefined
            );
    }
}
