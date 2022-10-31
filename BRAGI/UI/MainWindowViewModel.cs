using Avalonia.Threading;
using BRAGI.Bragi;
using BRAGI.Util;
using NAudio.CoreAudioApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BRAGI.UI;

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
    #endregion

    #region Audio
    public IEnumerable<MMDevice> _captureDevices = new List<MMDevice>();
    public IEnumerable<MMDevice> CaptureDevices
    {
        get => _captureDevices;
        private set
        {
            if (_captureDevices != value)
            {
                _captureDevices = value;
                OnPropertyChanged(nameof(CaptureDevices));
            }
        }
    }

    private MMDevice? _inputDevice;
    public MMDevice? InputDevice
    {
        get => _inputDevice;
        private set
        {
            if (_inputDevice != value)
            {
                _inputDevice = value;
                OnPropertyChanged(nameof(InputDevice));
            }
        }
    }

    private MMDevice? _outputDevice;
    public MMDevice? OutputDevice
    {
        get => _outputDevice;
        private set
        {
            if (_outputDevice != value)
            {
                _outputDevice = value;
                OnPropertyChanged(nameof(OutputDevice));
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
                OnPropertyChanged(nameof(InputGain));
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
                OnPropertyChanged(nameof(Volume));
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
                OnPropertyChanged(nameof(InputGate));
            }
        }
    }
    #endregion
    public MainWindowViewModel()
    {
        BragiAudio.SettingsChanged += BragiAudio_SettingsChanged;
        Bragi.Bragi.StateChanged += Bragi_StateChanged;
        CaptureDevices = Audio.ActiveCaptureDevices;
    }

    private void Bragi_StateChanged(object? sender, EventArgs e)
    {
        BragiLoaded = (Bragi.Bragi.Instance != null) && Bragi.Bragi.Instance.State == BRAGISTATE.INITIALIZED;
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            MainWindow.Instance!.Tabs.SelectedItem = (BragiLoaded) ? MainWindow.Instance.BragiTab : MainWindow.Instance.WaitingTab; 
        });
    }

    private void BragiAudio_SettingsChanged(object? sender, EventArgs e)
    {
        OutputDevice = BragiAudio.OutputDevice;
        InputDevice = BragiAudio.InputDevice;
        InputGain = (int)(BragiAudio.InputGain * 100);
        Volume = BragiAudio.Volume;
        InputGate = (int)(BragiAudio.InputGate * 100);
    }
}
