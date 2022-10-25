using BRAGI.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands
{
    enum LeaveRoomError
    {
        BRAGI_INITIALIZATION_ERROR
    }
    public interface LeaveRoomParameters
    {
        string RoomName { get; set; }
    }

    public static class CLeaveRoom
    {
        public static async Task<object> LeaveRoom(JObject parameters)
        {
            if (!JsonValidator.CheckParameters<LeaveRoomParameters>(parameters)) return "";
            if (Bragi.Instance.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)LeaveRoomError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
            return await Bragi.Instance.LeaveRoom((string)parameters["RoomName"]);
        }
    }
}
