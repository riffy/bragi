using BRAGI.Util;
using NAudio.CoreAudioApi;
using Newtonsoft.Json.Linq;
using OdinNative.Odin.Room;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands
{
    enum JoinRoomError
    {
        BRAGI_INITIALIZATION_ERROR,
        ROOM_JOIN_FAILURE
    }
    public interface JoinRoomParameters
    {
        string RoomName { get; set; }
        string TokenOrUserId { get; set; }   
    }

    public static class CJoinRoom
    {
        public static async Task<object> JoinRoom(JObject parameters)
        {
            if (!JsonValidator.CheckParameters<JoinRoomParameters>(parameters)) return "";
            if (Bragi.Instance.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)JoinRoomError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
            Room room = await Bragi.Instance.JoinRoom((string)parameters.GetValue("RoomName"), (string)parameters.GetValue("TokenOrUserId"));
            if (room == null) throw new CommandException((int)JoinRoomError.ROOM_JOIN_FAILURE, "Failed joining Room");
            return new Dictionary<string, object>()
            {
                ["ID"] = room.RoomId,
                ["Peers"] = room.GetRemotePeersIds(false),
                ["Self"] = room.Self,
                ["Config"] = room.Config
            };
        }
    }
}
