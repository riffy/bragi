using BRAGI.Bragi;
using BRAGI.Util;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Bragi.Bragi.Commands;

public class GetAudioDevices : BragiCommand<NoParameter>
{
    public override bool CheckParameters(JsonObject? param)
    {
        return true;
    }

    public async override Task<object> ExecuteInternal(NoParameter? param)
    {
        return await Audio.GetAudioDevices();
    }

    public override NoParameter? ParseParameters(JsonObject? param)
    {
        return null;
    }
}