using System.Text.Json;

namespace BRAGI.Bragi.Events;

public class BragiEvent
{
    public string Event { get; private set; }
    public object Data { get; private set; }
    public BragiEvent(string ev, object da)
    {
        Event = ev;
        Data = da;
    }
    /// <summary>
    /// Wrapper for the broadcast event
    /// </summary>
    /// <param name="ev"></param>
    /// <param name="data"></param>
    public static async void BroadcastEvent(string ev, object da)
    {
        Valhalla.Valhalla.Broadcast(JsonSerializer.Serialize(new BragiEvent(ev, da)));
    }
}
