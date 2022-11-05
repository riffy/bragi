using BRAGI.Util;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum JoinRoomError
{
    BRAGI_INITIALIZATION_ERROR,
    ROOM_JOIN_FAILURE
}
public class JoinRoomParameter : BragiParameter
{
    public string RoomName { get; private set; }
    public string TokenOrUserId { get; private set; }
    [OptionalParameter]

    public JoinRoomParameter(string roomName, string tokenOrUserId)
    {
        RoomName = roomName;
        TokenOrUserId = tokenOrUserId;
    }
}

public class JoinRoom : BragiCommand<JoinRoomParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        return JsonValidator.CheckParameters<JoinRoomParameter>(parameters);
    }

    public override JoinRoomParameter? ParseParameters(JsonObject? parameters)
    {
        if (!CheckParameters(parameters)) return null;
        return new JoinRoomParameter(
            (string)parameters!["RoomName"]!,
            (string)parameters!["TokenOrUserId"]!);
    }

    public async override Task<object> ExecuteInternal(JoinRoomParameter? parameters)
    {
        if (Bragi.Instance!.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)JoinRoomError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
        BragiRoom.BragiRoom? br = await BragiRoom.BragiRoom.JoinRoom(parameters!.RoomName, parameters!.TokenOrUserId);
        if (br == null || br.Room == null) throw new CommandException((int)JoinRoomError.ROOM_JOIN_FAILURE, "Failed joining Room");
        return new Dictionary<string, object?>()
        {
            ["Id"] = br.Room.RoomId,
            ["Peers"] = br.Room.GetRemotePeersIds(false),
            ["Self"] = await OdinParser.ParsePeer(br.Room.Self)
        };
    }
}