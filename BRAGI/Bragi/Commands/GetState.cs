using BRAGI.Util;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

public class GetState : BragiCommand<NoParameter>
{
    public override bool CheckParameters(JsonObject? param)
    {
        return true;
    }

    public async override Task<object> ExecuteInternal(NoParameter? param)
    {
        Dictionary<string, object?> result = new()
        {
            ["BragiInitialized"] = Bragi.Instance!.State == BRAGISTATE.INITIALIZED,
        };
        if (Bragi.Instance!.State == BRAGISTATE.INITIALIZED)
        {
            result["Audio"] = await BragiCommands.GetFunction(GetAudioSettings.Command).Execute(null);
        }
        return result;
    }

    public override NoParameter? ParseParameters(JsonObject? param)
    {
        return null;
    }
}
