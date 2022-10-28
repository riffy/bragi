using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRAGI.Bragi;

namespace BRAGI.Util;

public static class Audio
{
    /// <summary>
    /// Returns the sound devices connected in a dictionary with an ID (string) and their name.
    /// in: List of Soundinputs (micrphone)
    /// out: List of Soundoutputs (speaker, headset,...)
    /// </summary>
    /// <returns></returns>
    public static async Task<object> GetAudioDevices()
    {
        Dictionary<string, List<SimplifiedAudioDevice>> result = new()
        {
            ["In"] = new List<SimplifiedAudioDevice>(),
            ["Out"] = new List<SimplifiedAudioDevice>()
        };
        MMDeviceEnumerator enumerator = new();
        foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            SimplifiedAudioDevice? res = SimplifyMMDevice(endpoint);
            if (res != null) result["Out"].Add(res);
        }
        enumerator.Dispose();
        enumerator = new MMDeviceEnumerator();
        foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
        {
            SimplifiedAudioDevice? res = SimplifyMMDevice(endpoint);
            if (res != null) result["In"].Add(res);
        }
        enumerator.Dispose();
        return result;
    }

    /// <summary>
    /// Returns an MMDevice based on the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static MMDevice? GetAudioDeviceByID(string? id, DEVICETYPE dt)
    {
        MMDevice? result = null;
        MMDeviceEnumerator enumerator = new();
        foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints((dt == DEVICETYPE.IN) ? DataFlow.Capture : DataFlow.Render, DeviceState.Active))
        {
            if (endpoint.ID == id)
            {
                result = endpoint;
                break;
            }
        }

        enumerator.Dispose();
        return result;
    }

    /// <summary>
    /// Parses a given MMDevice to simplified version.
    /// </summary>
    /// <param name="device"></param>
    /// <returns>Simplified device or null of if not provided</returns>
    public static SimplifiedAudioDevice? SimplifyMMDevice(MMDevice? device)
    {
        if (device == null) return null;
        return new SimplifiedAudioDevice()
        {
            Id = device.ID,
            Name = device.DeviceFriendlyName
        };
    }

}

public class SimplifiedAudioDevice
{
    public string? Id { get; set; }
    public string? Name { get; set; }

    public SimplifiedAudioDevice() { }
}
