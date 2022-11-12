using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Threading.Tasks;
using BRAGI.Bragi;
using System;
using NAudio.CoreAudioApi.Interfaces;

namespace BRAGI.Util.AudioUtil;

public static class Audio
{
    #region Events
    private static readonly MMDeviceEnumerator DeviceEnum = new();
    private static AudioNotification? NotificationClient;
    public static event EventHandler<string>? OnDeviceAdded;
    public static event EventHandler<string>? OnDeviceRemoved;
    public static event EventHandler<DeviceStateChangedArgs>? OnDeviceStateChanged;
    #endregion
    public static IEnumerable<MMDevice> ActiveInputDevices
    {
        get
        {
            MMDeviceEnumerator enumerator = new();
            List<MMDevice> devices = new List<MMDevice>();
            foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                devices.Add(endpoint);
            }
            enumerator.Dispose();
            return devices;
        }
    }
    public static IEnumerable<SimplifiedAudioDevice> SimpleActiveInputDevices
    {
        get
        {
            List<SimplifiedAudioDevice> devices = new List<SimplifiedAudioDevice>();
            foreach (MMDevice aid in ActiveInputDevices)
            {
                SimplifiedAudioDevice? sad = SimplifyMMDevice(aid);
                if (sad != null) devices.Add(sad);
            }
            return devices;
        }
    }
    public static IEnumerable<MMDevice> ActiveOutputDevices
    {
        get
        {
            MMDeviceEnumerator enumerator = new();
            List<MMDevice> devices = new List<MMDevice>();
            foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                devices.Add(endpoint);
            }
            enumerator.Dispose();
            return devices;
        }
    }
    public static IEnumerable<SimplifiedAudioDevice> SimpleActiveOutputDevices
    {
        get
        {
            List<SimplifiedAudioDevice> devices = new List<SimplifiedAudioDevice>();
            foreach (MMDevice aod in ActiveOutputDevices)
            {
                SimplifiedAudioDevice? sad = SimplifyMMDevice(aod);
                if (sad != null) devices.Add(sad);
            }
            return devices;
        }
    }
    /// <summary>
    /// Returns the sound devices connected in a dictionary with an ID (string) and their name.
    /// in: List of Soundinputs (micrphone)
    /// out: List of Soundoutputs (speaker, headset,...)
    /// </summary>
    /// <returns></returns>
    public static async Task<object> GetAudioDevices()
    {
        Dictionary<string, IEnumerable<SimplifiedAudioDevice>> result = new()
        {
            ["In"] = SimpleActiveInputDevices,
            ["Out"] = SimpleActiveOutputDevices
        };
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
                 enumerator.EnumerateAudioEndPoints(dt == DEVICETYPE.IN ? DataFlow.Capture : DataFlow.Render, DeviceState.Active))
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
        return new SimplifiedAudioDevice(device.ID, device.DeviceFriendlyName);
    }
    /// <summary>
    /// Registers the Audio events to propagate the events
    /// </summary>
    public static void RegisterAudioEvents()
    {
        if (NotificationClient == null)
        {
            NotificationClient = new();
            DeviceEnum.RegisterEndpointNotificationCallback(NotificationClient);
        }
    }
    /// <summary>
    /// Generic Wrapper for Audio Device changes, including unplugging etc.
    /// </summary>
    public class AudioNotification : IMMNotificationClient
    {
        public void OnDeviceAdded(string pwstrDeviceId)
        {
            Console.WriteLine("OnDeviceAdded: {0}", pwstrDeviceId);
            if (Audio.OnDeviceAdded != null) Audio.OnDeviceAdded.Invoke(this, pwstrDeviceId);
        }

        public void OnDeviceRemoved(string deviceId)
        {
            Console.WriteLine("OnDeviceRemoved: {0}", deviceId);
            if (Audio.OnDeviceRemoved != null) Audio.OnDeviceRemoved.Invoke(this, deviceId);
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            Console.WriteLine("OnDeviceStateChanged: {0} ; {1}", deviceId, newState);
            if (Audio.OnDeviceStateChanged != null) Audio.OnDeviceStateChanged.Invoke(this, new DeviceStateChangedArgs(deviceId, newState));
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) { }
    }

    public class DeviceStateChangedArgs : EventArgs
    {
        public DeviceStateChangedArgs(string deviceId, DeviceState newState)
        {
            DeviceId = deviceId;
            NewState = newState;
        }

        public string DeviceId { get; set; }
        public DeviceState NewState { get; set; }
    }
}

public class SimplifiedAudioDevice
{
    public string Id { get; set; }
    public string Name { get; set; }

    public SimplifiedAudioDevice(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return Name!;
    }
}
