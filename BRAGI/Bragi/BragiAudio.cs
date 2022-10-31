using Avalonia.Input;
using BRAGI.Util;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Data;
using System.Text;

namespace BRAGI.Bragi;

/// <summary>
/// The static global static audio class
/// </summary>
public static class BragiAudio
{
    #region Events
    private static readonly MMDeviceEnumerator DeviceEnum = new();
    private static BragiAudioNotification? NotificationClient;
    public static event EventHandler? SettingsChanged;
    public static event EventHandler<BragiWaveInEventArgs>? InputDataAvailable;
    public static event EventHandler? OnDeviceAdded;
    public static event EventHandler? OnDeviceRemoved;
    public static event EventHandler? OnDeviceStateChanged;
    #endregion
    #region In/Output
    /// <summary>
    /// Speaker
    /// </summary>
    private static MMDevice? _outputDevice;
    public static MMDevice? OutputDevice
    {
        get { return _outputDevice; }
        private set { 
            _outputDevice = value;
        }
    }
    private static WaveOutEvent? _outputEvent;
    private static MixingSampleProvider? _outputMixer;

    /// <summary>
    /// Microphone
    /// </summary>
    private static WasapiCapture? _inputCapture;

    private static MMDevice? _inputDevice;
    public static MMDevice? InputDevice
    {
        get { return _inputDevice; }
        private set
        { 
            _inputDevice = value;
        }
    }

    public static readonly float DefaultInputGain = 0.5f;
    private static float _inputGain = DefaultInputGain;
    public static float InputGain
    {
        get { return _inputGain; }
        private set
        {
            _inputGain = value;
        }
    }

    public static readonly float DefaultInputGate = 0.1f;
    private static float _inputGate = DefaultInputGate;
    public static float InputGate
    {
        get { return _inputGate; }
        private set
        {
            _inputGate = value;
        }
    }
    
    #endregion
    #region Global Audio Settings
    /// <summary>
    /// Push To Talk
    /// </summary>
    public static readonly Key DefaultP2TKey = Key.None;
    private static Key _p2tKey = DefaultP2TKey;
    public static Key P2TKey 
    {
        get { return _p2tKey; }
        set
        {
            _p2tKey = value;
        }
    }
    public static bool UseP2T
    {
        get
        {
            return (_p2tKey != Key.None);
        }
    }

    /// <summary>
    /// Volume
    /// </summary>
    public static readonly int DefaultVolume = 100;
    private static int _volume = DefaultVolume;
    public static int Volume
    {
        get { return _volume; }
        private set {
            _volume = value;
        }
    }
    #endregion

    public static void ApplySettings(MMDevice inputDevice, MMDevice outputDevice, int volume = 100, Key p2TKey = Key.None)
    {
        Console.WriteLine("Applying new Settings {0} , {1}", Volume, P2TKey);
        InputDevice = inputDevice;
        OutputDevice = outputDevice;
        Volume = volume;
        P2TKey = p2TKey;
        //InitializeOutput();
        InitializeInput();
        RegisterEventCallback();

        if (SettingsChanged != null) SettingsChanged.Invoke(null, null!);
    }

    internal static void RegisterEventCallback()
    {
        if (NotificationClient == null)
        {
            NotificationClient = new BragiAudioNotification();
            DeviceEnum.RegisterEndpointNotificationCallback(NotificationClient);
        }
    }

    internal static void InitializeOutput()
    {
        if (_outputEvent != null)
        {
            _outputEvent.Dispose();
        }
        _outputEvent = new WaveOutEvent();

        if (_outputMixer == null) _outputMixer = new(new WaveFormat());
        _outputMixer.ReadFully = true;
    }

    internal static void InitializeInput()
    {
        DisposeInput();
        _inputCapture = new(InputDevice)
        {
            ShareMode = AudioClientShareMode.Shared,
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1)
        };
        _inputCapture.StartRecording();
        _inputCapture.DataAvailable += InputCapture_DataAvailable;
    }

    /// <summary>
    /// Takes the raw captured audio byte buffer and converts it to float buffer.
    /// WASAPI always records audio as IEEE floating point samples as determined above.
    /// So in the recorded buffer of the callback, every 4 bytes is a float.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void InputCapture_DataAvailable(object? sender, WaveInEventArgs e)
    {
        if (e == null) return;
        if (e.BytesRecorded <= 0) return;
        WaveBuffer buf = new(e.Buffer);
        bool gatePassed = false;
        foreach (float item in buf.FloatBuffer)
        {
            if (item > InputGate)
            {
                gatePassed = true;
                break;
            }
        }
        if (gatePassed && InputDataAvailable != null) 
        {
            InputDataAvailable.Invoke(sender, new BragiWaveInEventArgs(buf.FloatBuffer));
        }
    }

    public static void DisposeInput()
    {
        if (_inputCapture != null)
        {
            _inputCapture.DataAvailable -= InputCapture_DataAvailable;
            _inputCapture.StopRecording();
            _inputCapture.Dispose();
            _inputCapture = null;
        }
    }

    public class BragiAudioNotification : IMMNotificationClient
    {
        

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            Console.WriteLine("Audio Device Added: {0}", pwstrDeviceId);
        }

        public void OnDeviceRemoved(string deviceId)
        {
            Console.WriteLine("Audio Device Unplugged: {0}", deviceId);
            if (BragiAudio.OutputDevice != null && deviceId == BragiAudio.OutputDevice.ID)
            {
                Console.WriteLine("Audio Output was unplugged");
            }
            if (BragiAudio.InputDevice != null && deviceId == BragiAudio.InputDevice.ID)
            {
                if (deviceId == BragiAudio.InputDevice.ID)
                {
                    Console.WriteLine("Audio Input was unplugged");
                }
            }
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            Console.WriteLine("Audio Device State Changed: {0} , {1}", deviceId, newState);
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) { }
    }

}

public class BragiWaveInEventArgs : EventArgs
{
    private float[] buffer;
    public float[] Buffer => buffer;

    public BragiWaveInEventArgs(float[] buffer)
    {
        this.buffer = buffer;
    }
}