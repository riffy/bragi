using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BRAGI.Bragi.Commands;

public class Ping : BragiCommand<NoParameter>
{
    public override bool CheckParameters(JsonObject? parameters)
    {
        return true;
    }

    public async override Task<object> ExecuteInternal(NoParameter? parameters)
    {
        return "Pong";
    }

    public override NoParameter? ParseParameters(JsonObject? parameters)
    {
        return null;
    }
}
