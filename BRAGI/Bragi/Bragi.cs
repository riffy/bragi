using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BRAGI.Bragi.Commands;
using OdinNative.Odin;
using OdinNative.Odin.Room;

namespace BRAGI.Bragi;

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

public class Bragi
{

    /// <summary>
    /// Refers to the current state of Bragi.
    /// If an ODIN client is present, it is considered initialized.
    /// </summary>
    /// 
    private BRAGISTATE _state = BRAGISTATE.NOT_INITIALIZED;
    public BRAGISTATE State 
    { 
        get 
        {
            return _state;
        } 
        set
        {
            _state = value;
        }
    }
    /// <summary>
    /// The currently running Bragi Instance
    /// </summary>
    public static Bragi? Instance { get; private set; }
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
        }
    }

    /// <summary>
    /// The acutal ODIN client
    /// </summary>
    /// 
    public static OdinClient? Client { get; set; }
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
        State = BRAGISTATE.INITIALIZED;
    }

    /// <summary>
    /// Joins a room by the provided information. If the access key is set, the second parameter is interpreted as the token.
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="tokenOrUserID"></param>
    /// <returns></returns>
    public async Task<Room?> JoinRoom(string roomName, string tokenOrUserID)
    {
        Room? room = null;
        if (Client == null) return null;
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
    /// Leaves a room by the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>True on success, False on fail</returns>
    public async Task<bool> LeaveRoom(string name)
    {
        if (Client == null) return false;
        return await Client.LeaveRoom(name);
    }

    /// <summary>
    /// Performs a Cleanup sequence.
    /// Leaves all rooms the current ODIN client is connected.
    /// </summary>
    public void CleanUp()
    {
        if (State == BRAGISTATE.INITIALIZED &&
            Client != null)
        {
            Client.Rooms.FreeAll();
        }
        State = BRAGISTATE.NOT_INITIALIZED;
        Client = null;
    }

    public BragiState GetState()
    {
        return new BragiState()
        {
            State = State
        };
    }
}

/// <summary>
/// 
/// </summary>
public class BragiState
{
    public BRAGISTATE State { get; set; }

    public BragiState() { }
}
