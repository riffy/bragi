using System;
using System.Net;
using WebSocketSharp.Server;
using WebSocketSharp;
using BRAGI.Util;
using System.Linq;
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

namespace BRAGI.Valhalla;

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
public class Valhalla
{
    public static Valhalla? Instance;
    public static VALHALLASTATE State = VALHALLASTATE.CLOSED;
    /// <summary>
    /// Websocket Server and server properties
    /// </summary>
    public WebSocketServer? Server { get; private set; }
    public string? ValhallaOpenedTimestamp { get; set; }

    public bool ValhallaOpen { get; private set; }

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
                /* TODO
                HeroArrivedTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture);
                */
            }
        }
    }

    public Bragi.Bragi Bragi { get; private set; }
    public DateTime LastMessageReceived { get; set; }


    /// <summary>
    /// Bragi WebSocket
    /// </summary>
    public Valhalla()
    {
        Bragi = new Bragi.Bragi();
        BragiCommands.AssociateCommands();
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
            Instance.StartWebsocket();
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
    internal void StartWebsocket()
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
        Thread t = new(new ThreadStart(CheckServerState));
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
        if (Server != null)
        {
            Server.Stop();
            Console.WriteLine("Valhalla stopped");
        }
    }
}