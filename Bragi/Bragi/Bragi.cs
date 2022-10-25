using NAudio.CoreAudioApi;
using OdinNative.Odin.Room;
using OdinNative.Odin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BRAGI.Bragi.Commands;

namespace BRAGI.Bragi
{
    public enum DEVICETYPE
    {
        IN,
        OUT
    }
    /// <summary>
    /// Represents the state of Bragi, including Audio
    /// </summary>
    public enum BRAGISTATE
    {
        NOT_INITIALIZED, // Audio Settings are missing
        INITIALIZED // Initial Audio Settings are set
    }

    public class Bragi : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BRAGISTATE State { get
            {
                return (Client == null) ? BRAGISTATE.NOT_INITIALIZED : BRAGISTATE.INITIALIZED;
            } }
        public static Bragi Instance { get; private set; }
        /// <summary>
        /// Settings for Bragi, the audio output etc.
        /// </summary>
        private BragiSettings _settings = new BragiSettings();
        public BragiSettings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The acutal ODIN client
        /// </summary>
        /// 

        public static OdinClient Client { get; set; }
        public OdinClient GetOdinClient() { return Client; }
        public Bragi() {
            if (Instance != null) throw new Exception();
            Instance = this;
        }
       
        /// <summary>
        /// Initializes the ODIN client using the defaults.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Initialize()
        {
            if (State == BRAGISTATE.INITIALIZED) throw new Exception("ODIN Client already initialized.");
            Client = new OdinClient(OdinDefaults.Server, OdinDefaults.AccessKey);
        }

        /// <summary>
        /// Joins a room by the provided information. If the access key is set, the second parameter is interpreted as the token.
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="tokenOrUserID"></param>
        /// <returns></returns>
        public async Task<Room> JoinRoom(string roomName, string tokenOrUserID)
        {
            Room room = null;
            if (string.IsNullOrEmpty(Client.AccessKey))
            {
                Console.WriteLine("Joining Room {0} by Token {1}", roomName, tokenOrUserID);
                room = await Client.JoinNamedRoom(roomName, tokenOrUserID);
            }
            else
            {
                Console.WriteLine("Joining Room {0} with UserID {1} using AccessKey", roomName, tokenOrUserID);
                room = await Client.JoinRoom(roomName, tokenOrUserID);
            }
            if (room != null) RegisterRoomEvents(room);
            return room;
        }

        /// <summary>
        /// Registers the Room events for a given room.
        /// </summary>
        /// <param name="room"></param>
        internal void RegisterRoomEvents(Room room)
        {
            room.OnConnectionStateChanged += Room_OnConnectionStateChanged;
            room.OnMediaActiveStateChanged += Room_OnMediaActiveStateChanged;
            room.OnPeerJoined += Room_OnPeerJoined;
            room.OnPeerLeft += Room_OnPeerLeft;
        }

        private void Room_OnPeerLeft(object sender, PeerLeftEventArgs e)
        {
            Console.WriteLine("Room_OnPeerLeft:");
            Console.WriteLine(sender);
            Console.WriteLine(e);
        }

        private void Room_OnPeerJoined(object sender, PeerJoinedEventArgs e)
        {
            Console.WriteLine("Room_OnPeerJoined:");
            Console.WriteLine(sender);
            Console.WriteLine(e);
        }

        private void Room_OnMediaActiveStateChanged(object sender, MediaActiveStateChangedEventArgs e)
        {
            Console.WriteLine("Room_OnMediaActiveStateChanged:");
            Console.WriteLine(sender);
            Console.WriteLine(e);
        }

        private void Room_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            Console.WriteLine("Room_OnConnectionStateChanged:");
            Console.WriteLine(sender);
            Console.WriteLine(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> LeaveRoom(string name)
        {
            return await Client.LeaveRoom(name);
        }
        public void CleanUp()
        {
            if (State == BRAGISTATE.INITIALIZED)
            {
                Client.Rooms.FreeAll();
            }
            Client = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BragiState GetState()
        {
            return new BragiState()
            {
                State = State
            };
        }
    }

    public class BragiState
    {
        public BRAGISTATE State { get; set; }

        public BragiState() { }
    }
}
