using BRAGI.Util.AudioUtil;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using SharpHook.Native;
using System;

namespace BRAGI.Bragi;

/// <summary>
/// The static global static audio class
/// </summary>
public static class BragiAudio
{
    #region Events
    private static bool IsTalking { get; set; } = false;
    public static event EventHandler? SettingsChanged;
    public static event EventHandler<BragiWaveInEventArgs>? InputDataAvailable;
    #endregion
    #region Output
    /// <summary>
    /// Speaker
    /// </summary>
    public static MMDevice? OutputDevice
    {
        get 
        { 
            return Audio.GetAudioDeviceByID(OutputDeviceId, DEVICETYPE.OUT); 
        }
    }
    public static string? OutputDeviceId { get; private set; }

    public static AsioOut? AsioOut { get; set; }

    /// <summary>
    /// Volume
    /// </summary>
    public static readonly int DefaultVolume = 100;
    private static int _volume = DefaultVolume;
    public static int Volume
    {
        get { return _volume; }
        private set
        {
            _volume = value;
        }
    }
    #endregion
    #region Input
    /// <summary>
    /// Microphone
    /// </summary>
    private static WasapiCapture? _inputCapture;
    public static MMDevice? InputDevice
    {
        get
        {
            return Audio.GetAudioDeviceByID(InputDeviceId, DEVICETYPE.IN);
        }
    }
    public static string? InputDeviceId { get; private set; }

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
    public static readonly KeyCode DefaultP2TKey = KeyCode.VcUndefined;
    private static KeyCode _p2tKey = DefaultP2TKey;
    public static KeyCode P2TKey 
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
            return (_p2tKey != KeyCode.VcUndefined);
        }
    }
    #endregion
    /// <summary>
    /// Applies Audio Settings
    /// </summary>
    /// <param name="inputDeviceId"></param>
    /// <param name="outputDeviceId"></param>
    /// <param name="volume"></param>
    /// <param name="p2TKey"></param>
    /// <param name="gain"></param>
    /// <param name="gate"></param>
    public static void ApplySettings(string inputDeviceId, string outputDeviceId,
        int volume = 100, KeyCode p2TKey = KeyCode.VcUndefined, float gain = 0.5f, float gate = 0.5f)
    {
        Console.WriteLine("Applying new Settings {0} , {1}, {2}, {3}", Volume, P2TKey, gain, gate);
        P2TKey = p2TKey;
        InputGate = gate;
        if (ApplyInputCaptureSettings(inputDeviceId, gain))
        {
            InitializeCapture();
        }

        Volume = volume;
        if (ApplyOutputSettings(outputDeviceId))
        {
            InitializeOutput();
        }
        if (SettingsChanged != null) SettingsChanged.Invoke(null, null!);
    }
    /// <summary>
    /// Resets the audio settings
    /// </summary>
    public static void ResetSettings()
    {
        DisposeCapture();
        DisposeOutput();
        InputDeviceId = null;
        OutputDeviceId = null;
        P2TKey = DefaultP2TKey;
        InputGate = DefaultInputGate;
        InputGain = DefaultInputGain;
    }
    #region Output
    internal static bool ApplyOutputSettings(string outputDeviceId)
    {
        bool result = false;
        if (outputDeviceId != OutputDeviceId)
        {
            OutputDeviceId = outputDeviceId;
            result = true;
        }
        return result;
    }
    internal static void InitializeOutput()
    {
        DisposeOutput();
        /*
        if (_outputEvent != null)
        {
            _outputEvent.Dispose();
        }
        _outputEvent = new WaveOutEvent();

        if (_outputMixer == null) _outputMixer = new(new WaveFormat());
        _outputMixer.ReadFully = true;
        */
    }
    internal static void DisposeOutput()
    {

    }
    /// <summary>
    /// Takes an audio stream and mixes it in based on the provided config
    /// </summary>
    public static void MixinAudioStream()
    {

    }
    #endregion
    #region Capture
    /// <summary>
    /// Applies the input settings that correspond to the capture. Returns true if a change in any value is detected
    /// </summary>
    /// <returns></returns>
    internal static bool ApplyInputCaptureSettings(string inputDeviceId, float gain)
    {
        bool result = false;
        if (inputDeviceId != InputDeviceId)
        {
            InputDeviceId = inputDeviceId;
            result = true;
        }
        if (gain != InputGain)
        {
            InputGain = gain;
            result = true;
        }
        return result;
    }
    /// <summary>
    /// Initializees Capture with the selected Input Device.
    /// </summary>
    internal static void InitializeCapture()
    {
        DisposeCapture();
        if (InputDevice != null)
        {
            _inputCapture = new(InputDevice)
            {
                ShareMode = AudioClientShareMode.Shared,
                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1)
            };
            _inputCapture.StartRecording();
            _inputCapture.DataAvailable += InputCapture_DataAvailable;
            InputDevice.AudioEndpointVolume.MasterVolumeLevelScalar = InputGain;
        }
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
        if (InputDevice == null || InputDevice.AudioMeterInformation.MasterPeakValue < InputGate) return;
        if (InputDataAvailable != null)
        {
            InputDataAvailable.Invoke(sender, new BragiWaveInEventArgs(new WaveBuffer(e.Buffer).FloatBuffer));
        }
    }
    /// <summary>
    /// Disposes the capture
    /// </summary>
    internal static void DisposeCapture()
    {
        if (_inputCapture != null)
        {
            _inputCapture.DataAvailable -= InputCapture_DataAvailable;
            _inputCapture.StopRecording();
            _inputCapture.Dispose();
            _inputCapture = null;
        }
    }
    #endregion
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