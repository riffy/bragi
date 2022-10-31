using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum SetRoomConfigError
{
    BRAGI_INITIALIZATION_ERROR,
    UNKNOWN_ROOM
}

public interface BragiRoomConfig
{

}

public class SetRoomConfigParameter : BragiParameter
{
    public string RoomName { get; private set; }

    public SetRoomConfigParameter(string roomName)
    {
        RoomName = roomName;
    }
}

public class SetRoomConfig : BragiCommand<SetRoomConfigParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        throw new NotImplementedException();
    }

    public override Task<object> ExecuteInternal(SetRoomConfigParameter? param)
    {
        throw new NotImplementedException();
    }

    public override SetRoomConfigParameter? ParseParameters(JsonObject? parameters)
    {
        throw new NotImplementedException();
    }
}
