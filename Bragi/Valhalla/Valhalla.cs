using System;
using System.Net;
using WebSocketSharp.Server;
using WebSocketSharp;
using BRAGI.Util;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;
using OdinNative;
using OdinNative.Odin;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BRAGI.Bragi;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace BRAGI.Valhalla
{
    /// <summary>
    /// Represents the state of the valhalla instance
    /// </summary>
    public enum VALHALLASTATE
    {
        CLOSED,
        RUNNING,
        RUNNING_CLIENT_CONNECTED
    }

    /// <summary>
    /// Valhalla is the Websocket server where the clients connect to and are greeted by Bragi.
    /// </summary>
    public class Valhalla : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static Valhalla Instance;
        public static VALHALLASTATE State = VALHALLASTATE.CLOSED;
        /// <summary>
        /// Websocket Server and server properties
        /// </summary>
        public WebSocketServer Server { get; private set; }
        private bool _valhallaOpen = false;
        public bool ValhallaOpen
        {
            get { return _valhallaOpen; }
            set
            {
                State = value ? VALHALLASTATE.RUNNING : VALHALLASTATE.CLOSED;
                _valhallaOpen = value;
                OnPropertyChanged();
            }
        }
        public string ValhallaOpenedTimestamp { get; set; }

        /// <summary>
        /// Hero / Client
        /// </summary>
        private bool _heroArrived = false;
        public bool HeroArrived
        {
            get { return _heroArrived; }
            set
            {
                if (State > VALHALLASTATE.CLOSED)
                {
                    State = value ? VALHALLASTATE.RUNNING_CLIENT_CONNECTED : VALHALLASTATE.RUNNING;
                }
                _heroArrived = value;
                if (value)
                {
                    HeroArrivedTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                        CultureInfo.InvariantCulture);
                }
                OnPropertyChanged();
            }
        }
        public string HeroArrivedTimestamp { get; set; }

        public Bragi.Bragi Bragi { get; private set; }
        public DateTime LastMessageReceived { get; set; }
        public string ValhallaInfo
        {
            get
            {
                if (LastMessageReceived == DateTime.MinValue || this.LastMessageReceived == null)
                {
                    return "No Message received";
                }
                else
                {
                    return "Last: " + LastMessageReceived.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture);
                }
            }
        }


        /// <summary>
        /// Bragi WebSocket
        /// </summary>
        public Valhalla()
        {
            Bragi = new Bragi.Bragi();
            BragiCommands.AssociateCommands(this);
        }
        /// <summary>
        /// Starts a Valhalla Instance
        /// </summary>
        /// <returns></returns>
        public static Valhalla Start()
        {
            if (Instance == null)
            {
                Instance = new Valhalla();
                Instance._Start();
                return Instance;
            }
            else
            {
                throw new Exception("A WebSocket Server is already running...");
            }
        }
        /// <summary>
        /// Starts the Websocket
        /// </summary>
        public void _Start()
        {
            Server = new WebSocketServer(WebSocketDefaults.host, WebSocketDefaults.port);
            Console.WriteLine("Starting Valhalla, opening gate {0}:{1} ...", WebSocketDefaults.host, WebSocketDefaults.port);
            Server.AddWebSocketService<BragiBehavior>("/");
            Server.Start();
            ValhallaOpenedTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                CultureInfo.InvariantCulture);
            StartCheckingServerState();
        }
        /// <summary>
        /// Starts the checking thread for the websocket server state
        /// </summary>
        internal void StartCheckingServerState()
        {
            Thread t = new Thread(new ThreadStart(CheckServerState));
            t.Start();
        }
        /// <summary>
        /// Loops continously and checks if the server is listenening
        /// </summary>
        internal void CheckServerState()
        {
            while (Server != null)
            {
                ValhallaOpen = Server.IsListening;
                if (!Server.IsListening)
                {
                    break;
                }
                Thread.Sleep(250);
            }
            Console.WriteLine("Websocket Server was closed during check.");
            CleanUp();
        }
        /// <summary>
        /// Performs cleanup
        /// </summary>
        internal void CleanUp()
        {
            Console.WriteLine("Performing CleanUp...");
            HeroArrived = false;
            Server = null;
        }
        /// <summary>
        /// Stops the Websocket Server
        /// </summary>
        public void Stop()
        {
            Server.Stop();
            Console.WriteLine("Valhalla stopped");
        }

        /// <summary>
        /// Returns a pong.
        /// </summary>
        /// <returns></returns>
        public async Task<object> Pong(JObject parameters)
        {
            return "Pong";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    internal class ValhallaInfo
    {
        public DateTime LastMessageReceived { get; set; }
        public VALHALLASTATE State { get; set; }

        internal ValhallaInfo() { }
    }
}
