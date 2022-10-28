using Avalonia.Input;
using NAudio.CoreAudioApi;
using System;

namespace BRAGI.Bragi;

public class BragiSettings
{

    /// <summary>
    /// Devices
    /// </summary>
    private MMDevice? _outputDevice;
    public MMDevice? OutputDevice
    {
        get { return _outputDevice; }
        set { 
            _outputDevice = value;
        }
    }
    private MMDevice? _inputDevice;

    public MMDevice? InputDevice
    {
        get { return _inputDevice; }
        set { 
            _inputDevice = value;
        }
    }
    /// <summary>
    /// Push To Talk
    /// </summary>
    public static Key defaultP2TKey = Key.None;

    private Key _p2tKey = defaultP2TKey;
    public Key P2TKey 
    {
        get { return _p2tKey; }
        set
        {
            _p2tKey = value;
        }
    }
    public string PushToTalkInfo
    {
        get
        {
            if (!UseP2T) return "Inactive";
            if (P2TKey == Key.None) return "Unset";
            return P2TKey.ToString();
        }
    }
    public bool UseP2T
    {
        get
        {
            return (_p2tKey != Key.None);
        }
    }

    public static int defaultVolume = 100;

    private int _volume = defaultVolume;
    public int Volume
    {
        get { return _volume; }
        set {
            _volume = value;
        }
    }

    public BragiSettings() {}

    public void ApplySettings(MMDevice InputDevice, MMDevice OutputDevice, int Volume = 100, Key PushToTalkHotkey = Key.None)
    {
        Console.WriteLine("Applying new Settings {0} , {1}", Volume, PushToTalkHotkey);
        this.InputDevice = InputDevice;
        this.OutputDevice = OutputDevice;
        this.Volume = Volume;
        this.P2TKey = PushToTalkHotkey;
    }
}
