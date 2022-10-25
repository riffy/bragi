using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BRAGI.Bragi
{
    public class BragiSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Devices
        /// </summary>
        private MMDevice _outputDevice;
        public MMDevice OutputDevice
        {
            get { return _outputDevice; }
            set { 
                _outputDevice = value;
                OnPropertyChanged();
            }
        }
        private MMDevice _inputDevice;

        public MMDevice InputDevice
        {
            get { return _inputDevice; }
            set { 
                _inputDevice = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Push To Talk
        /// </summary>
        private Key _pushToTalkHotkey = Key.None;
        public Key PushToTalkHotkey 
        {
            get { return _pushToTalkHotkey; }
            set
            {
                _pushToTalkHotkey = value;
                OnPropertyChanged();
            }
        }
        public string PushToTalkInfo
        {
            get
            {
                if (!UsePushToTalk) return "Inactive";
                if (PushToTalkHotkey == Key.None) return "Unset";
                return PushToTalkHotkey.ToString();
            }
        }
        public bool UsePushToTalk
        {
            get
            {
                return (_pushToTalkHotkey != Key.None);
            }
        }

        private int _volume = 100;
        public int Volume
        {
            get { return _volume; }
            set {
                _volume = value;
                OnPropertyChanged();
            }
        }

        public BragiSettings() {}

        public void ApplySettings(MMDevice InputDevice, MMDevice OutputDevice, int Volume = 100, Key PushToTalkHotkey = Key.None)
        {
            Console.WriteLine("Applying new Settings {0} , {1}", Volume, PushToTalkHotkey);
            this.InputDevice = InputDevice;
            this.OutputDevice = OutputDevice;
            this.Volume = Volume;
            this.PushToTalkHotkey = PushToTalkHotkey;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
