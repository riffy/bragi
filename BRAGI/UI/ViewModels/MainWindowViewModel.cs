using Avalonia.Data.Converters;
using Avalonia.Threading;
using BRAGI.Bragi;
using BRAGI.Bragi.Commands;
using BRAGI.Util;
using NAudio.CoreAudioApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
    public bool TestModeActive
    {
        get { return _toggleTestLabel != StartTestLabel; }
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
    private int _inputGain;
    public int InputGain
    {
        get => _inputGain;
        set
        {
            if (_inputGain != value)
            {
                _inputGain = value;
                ChangeDetected = true;
                OnPropertyChanged(nameof(InputGain));
            }
        }
    }
    private int _inputGate;
    public int InputGate
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
    public MainWindowViewModel()
    {
        BragiAudio.SettingsChanged += BragiAudio_SettingsChanged;
        Bragi.Bragi.StateChanged += Bragi_StateChanged;
        Audio.OnDeviceAdded += Audio_OnDeviceAdded;
        Audio.OnDeviceRemoved += Audio_OnDeviceRemoved;
        Audio.OnDeviceStateChanged += Audio_OnDeviceStateChanged;
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
                    (float)InputGain / 100,
                    (float)InputGate / 100)
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
        Console.WriteLine("Starting input Test");
        ToggleTestLabel = EndTestLabel;
    }
    private async void StopInputTest()
    {
        Console.WriteLine("Stopping input Test");
        ToggleTestLabel = StartTestLabel;
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
        InputGain = (int)(BragiAudio.InputGain * 100);
        Volume = BragiAudio.Volume;
        InputGate = (int)(BragiAudio.InputGate * 100);
        ChangeDetected = false;
    }
}