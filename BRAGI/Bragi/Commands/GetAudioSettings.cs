using BRAGI.Util.AudioUtil;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

enum GetAudioError
{
    BRAGI_INITIALIZATION_ERROR
}

public class GetAudioSettings : BragiCommand<NoParameter>
{
    
    public override bool CheckParameters(JsonObject? param)
    {
        return true;
    }

    public async override Task<object> ExecuteInternal(NoParameter? param)
    {
        if (Bragi.Instance!.State != BRAGISTATE.INITIALIZED) throw new CommandException((int)GetAudioError.BRAGI_INITIALIZATION_ERROR, "Bragi not initialized");
        Dictionary<string, object?> result = new()
        {
            ["In"] = Audio.SimplifyMMDevice(BragiAudio.InputDevice),
            ["Out"] = Audio.SimplifyMMDevice(BragiAudio.OutputDevice),
            ["Volume"] = BragiAudio.Volume,
            ["PushToTalkKey"] = BragiAudio.P2TKey,
            ["InputGain"] = BragiAudio.InputGain,
            ["InputGate"] = BragiAudio.InputGate
        };
        return result;
    }

    public override NoParameter? ParseParameters(JsonObject? param)
    {
        return null;
    }
}
