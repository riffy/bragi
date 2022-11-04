using BRAGI.Bragi.Commands;
using System;
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
    /// Registers certain events that shall trigger a broadcast
    /// </summary>
    public static void RegisterEventsForBroadcast()
    {
        BragiAudio.SettingsChanged += BragiAudio_SettingsChanged;
    }

    private static async void BragiAudio_SettingsChanged(object? sender, EventArgs e)
    {
        BroadcastEvent("AudioSettingsChanged", await new GetAudioSettings().ExecuteInternal(new NoParameter()));
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
