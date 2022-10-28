using OdinNative.Odin.Peer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Util;

public static class OdinParser
{
    public static async Task<object?> ParsePeer(Peer p)
    {
        if (p == null) return null;
        Dictionary<string, object> result = new()
        {
            ["Id"] = p.Id,
            ["RoomName"] = p.RoomName,
            ["UserId"] = p.UserId
        };
        return result;
    }
}
