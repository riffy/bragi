using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using BRAGI.Bragi;
using BRAGI.Bragi.Commands;
using BRAGI.Util;
using BRAGI.Util.AudioUtil;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;

namespace BRAGI.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region State
    private bool _bragiLoaded = false;
    public bool BragiLoaded
    {
        get => _bragiLoaded;
        private set
        {
            if (_bragiLoaded != value)
            {
                _bragiLoaded = value;
                OnPropertyChanged(nameof(BragiLoaded));
            }
        }
    }

    private bool _changeDetected = false;
    public bool ChangeDetected
    {
        get => _changeDetected;
        private set
        {
            _changeDetected = value;
            OnPropertyChanged(nameof(ChangeDetected));
        }
    }
    private static string StartTestLabel = "Start Test";
    private static string EndTestLabel = "Stop Test";
    private string _toggleTestLabel = StartTestLabel;
    public string ToggleTestLabel
    {
        get => _toggleTestLabel;
        private set
        {
            _toggleTestLabel = value;
            OnPropertyChanged(nameof(ToggleTestLabel));
        }
    }
    #endregion
    #region Audio Output
    //public SimplifiedAudioDevice[] _outputDevices = Array.Empty<SimplifiedAudioDevice>();
    public IEnumerable<SimplifiedAudioDevice> _outputDevices = Array.Empty<SimplifiedAudioDevice>();
    public IEnumerable<SimplifiedAudioDevice> OutputDevices
    {
        get => _outputDevices;
        private set
        {
            if (_outputDevices != value)
            {
                _outputDevices = value;
                OnPropertyChanged(nameof(OutputDevices));
            }
        }
    }

    private SimplifiedAudioDevice? _selectedOutputDevice;
    public SimplifiedAudioDevice? SelectedOutputDevice
    {
        get => _selectedOutputDevice;
        private set
        {
            if (_selectedOutputDevice != value)
            {
                _selectedOutputDevice = value;
                ChangeDetected = true;
                OnPropertyChanged(nameof(SelectedOutputDevice));
            }
        }
    }

    private int _volume;
    public int Volume
    {
        get => _volume;
        set
        {
            if (_volume != value)
            {
                _volume = value;
                ChangeDetected = true;
                OnPropertyChanged(nameof(Volume));
            }
        }
    }
    #endregion
    #region Audio Input
    public IEnumerable<SimplifiedAudioDevice> _inputDevices = Array.Empty<SimplifiedAudioDevice>();
    public IEnumerable<SimplifiedAudioDevice> InputDevices
    {
        get => _inputDevices;
        private set
        {
            if (_inputDevices != value)
            {
                _inputDevices = value;
                OnPropertyChanged(nameof(InputDevices));
            }
        }
    }
    private SimplifiedAudioDevice? _selectedInputDevice;
    public SimplifiedAudioDevice? SelectedInputDevice
    {
        get => _selectedInputDevice;
        private set
        {
            if (_selectedInputDevice != value)
            {
                _selectedInputDevice = value;
                ChangeDetected = true;
                OnPropertyChanged(nameof(SelectedInputDevice));
            }
        }
    }
    private float _inputGain;
    public float InputGain
    {
        get => _inputGain;
        set
        {
            if (_inputGain != value)
            {
                _inputGain = value;
                ChangeDetected = true;
                if (TestModeActive && SelectedTestDevice != null)
                {
                    SelectedTestDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value;
                }
                OnPropertyChanged(nameof(InputGain));
            }
        }
    }
    private float _inputGate;
    public float InputGate
    {
        get => _inputGate;
        set
        {
            if (_inputGate != value)
            {
                _inputGate = value;
                ChangeDetected = true;
                OnPropertyChanged(nameof(InputGate));
            }
        }
    }
    #endregion
    #region InputTest
    public bool TestModeActive
    {
        get { return _capture != null; }
    }
    private float _peak;
    public float Peak
    {
        get => _peak;
        set
        {
            if (_peak != value)
            {
                _peak = value;
                OnPropertyChanged(nameof(Peak));
            }
        }
    }
    private WasapiCapture? _capture;
    public MMDevice? SelectedTestDevice { get; private set; }
    private readonly SynchronizationContext synchronizationContext;
    #endregion
    public MainWindowViewModel()
    {
        synchronizationContext = SynchronizationContext.Current!;
        BragiAudio.SettingsChanged += BragiAudio_SettingsChanged;
        Bragi.Bragi.StateChanged += Bragi_StateChanged;
        Audio.OnDeviceAdded += Audio_OnDeviceAdded;
        Audio.OnDeviceRemoved += Audio_OnDeviceRemoved;
        Audio.OnDeviceStateChanged += Audio_OnDeviceStateChanged;
        Valhalla.Valhalla.HeroLeftValhalla += Valhalla_HeroLeftValhalla;
    }
    public async void SaveSettings()
    {
        if (SelectedOutputDevice == null || !ValidateAudioDevice(SelectedOutputDevice, DEVICETYPE.OUT))
        {
            await MessageBox.Show(MainWindow.Instance!, "Failure while retrieving selected output device.", "Unknown Output Device", MessageBox.MessageBoxButtons.Ok);
        }
        else if (SelectedInputDevice == null || !ValidateAudioDevice(SelectedInputDevice, DEVICETYPE.IN))
        {
            await MessageBox.Show(MainWindow.Instance!, "Failure while retrieving selected input device.", "Unknown Input Device", MessageBox.MessageBoxButtons.Ok);
        }
        else
        {
            await new SetAudioSettings().ExecuteInternal(
                new AudioSettingsParameter(
                    SelectedInputDevice.Id,
                    SelectedOutputDevice.Id,
                    Volume,
                    BragiAudio.P2TKey,
                    (float)Math.Round(InputGain * 100f) / 100f,
                    (float)Math.Round(InputGate * 100f) / 100f)
                );
            Console.WriteLine(BragiAudio.InputGain);
        }
    }
    public async void ToggleTest()
    {
        if (!TestModeActive)
        {
            StartInputTest();
        }
        else
        {
            StopInputTest();
        }
    }
    private async void StartInputTest()
    {
        DisposeTest();
        if (SelectedInputDevice == null || !ValidateAudioDevice(SelectedInputDevice, DEVICETYPE.IN))
        {
            await MessageBox.Show(MainWindow.Instance!, "Failure while retrieving selected input device.", "Unknown Input Device", MessageBox.MessageBoxButtons.Ok);
        }
        else
        {
            Console.WriteLine("Starting Input Test");
            ToggleTestLabel = EndTestLabel;
            SelectedTestDevice = Audio.GetAudioDeviceByID(SelectedInputDevice.Id, DEVICETYPE.IN);
            if (SelectedTestDevice != null)
            {
                _capture = new(SelectedTestDevice)
                {
                    ShareMode = AudioClientShareMode.Shared,
                    WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1)
                };
                _capture.StartRecording();
                _capture.DataAvailable += UpdatePeakMeter;
                SelectedTestDevice.AudioEndpointVolume.MasterVolumeLevelScalar = InputGain;
            }
        }

    }
    private void UpdatePeakMeter(object? sender, WaveInEventArgs e)
    {
        // can't access this on a different thread from the one it was created on, so get back to GUI thread
        synchronizationContext.Post((s) =>
        {
            Peak = SelectedTestDevice!.AudioMeterInformation.MasterPeakValue;
            if (Peak >= InputGate)
            {
                synchronizationContext.Post(s => MainWindow.Instance!.PeakProgressBar.Foreground = Brushes.DarkOrange, null);
            }
            else
            {
                synchronizationContext.Post(s => MainWindow.Instance!.PeakProgressBar.Foreground = Brushes.LightGray, null);
            }
        }, null);
    }
    private async void StopInputTest()
    {
        Console.WriteLine("Stopping Input Test");
        DisposeTest();
        ToggleTestLabel = StartTestLabel;
        Peak = 0f;
    }
    private void DisposeTest()
    {
        if (_capture != null)
        {
            _capture.StopRecording();
            _capture.Dispose();
            _capture = null;
        }
    }
    private bool ValidateAudioDevice(SimplifiedAudioDevice? sad, DEVICETYPE dt)
    {
        if (sad == null) return false;
        MMDevice? dev = Audio.GetAudioDeviceByID(sad.Id, dt);
        if (dev == null) return false;
        return true;
    }
    private void Audio_OnDeviceStateChanged(object? sender, Audio.DeviceStateChangedArgs e)
    {
        ReadAudioAndUpdate();
    }
    private void Audio_OnDeviceRemoved(object? sender, string e)
    {
        ReadAudioAndUpdate();
    }
    private void Audio_OnDeviceAdded(object? sender, string e)
    {
        ReadAudioAndUpdate();
    }
    private void Bragi_StateChanged(object? sender, EventArgs e)
    {
        BragiLoaded = Bragi.Bragi.Instance != null && Bragi.Bragi.Instance.State == BRAGISTATE.INITIALIZED;
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            MainWindow.Instance!.Tabs.SelectedItem = BragiLoaded ? MainWindow.Instance.BragiTab : MainWindow.Instance.WaitingTab;
        });
        ReadAudioAndUpdate();
    }
    private void BragiAudio_SettingsChanged(object? sender, EventArgs e)
    {
        ReadAudioAndUpdate();
    }

    /// <summary>
    /// Reads the Audio devices, settings etc and updates the MVVMs
    /// Resets the changes done on the UI.
    /// </summary>
    public void ReadAudioAndUpdate()
    {
        InputDevices = Audio.SimpleActiveInputDevices;
        OutputDevices = Audio.SimpleActiveOutputDevices;
        if (BragiAudio.OutputDevice != null)
        {
            SelectedOutputDevice = OutputDevices.First(device => device.Id == BragiAudio.OutputDeviceId);
        }
        if (BragiAudio.InputDevice != null)
        {
            SelectedInputDevice = InputDevices.First(device => device.Id == BragiAudio.InputDeviceId);
        }
        InputGain = BragiAudio.InputGain;
        Volume = BragiAudio.Volume;
        InputGate = BragiAudio.InputGate;
        ChangeDetected = false;
    }

    private void Valhalla_HeroLeftValhalla(object? sender, EventArgs e)
    {
        if (TestModeActive)
        {
            StopInputTest();
        }
    }

}