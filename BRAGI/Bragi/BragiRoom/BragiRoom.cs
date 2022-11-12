using BRAGI.Bragi.Commands;
using BRAGI.Bragi.Events;
using BRAGI.Util;
using NAudio.Wave;
using OdinNative.Odin.Room;
using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BRAGI.Bragi.BragiRoom;

public class BragiRoom : IDisposable
{
    public static readonly Dictionary<string, BragiRoom> Collection = new();
    public Room? Room { get; private set; }
    public SetRoomConfigParameter Config { get; private set; }
    public BragiRoom(Room r, SetRoomConfigParameter config)
    {
        Room = r;
        InitializeRoom();
        Collection.Add(r.RoomId, this);
        Config = config;
    }

    /// <summary>
    /// Leaves a room by the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>True on success, False on fail</returns>
    public static async Task<bool> LeaveRoom(string name)
    {
        if (!Collection.ContainsKey(name)) return false;
        return await Collection[name].LeaveAndDispose();
    }
    /// <summary>
    /// Joins a room by the provided information. If the access key is set, the second parameter is interpreted as the token.
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="tokenOrUserID"></param>
    /// <returns></returns>
    public static async Task<BragiRoom?> JoinRoom(string roomName, string tokenOrUserID, SetRoomConfigParameter? srcp)
    {
        Room? room = null;
        if (Bragi.Client == null) return null;
        if (string.IsNullOrEmpty(Bragi.Client.AccessKey))
        {
            Console.WriteLine("Joining Room {0} by Token {1}", roomName, tokenOrUserID);
            room = await Bragi.Client.JoinNamedRoom(roomName, tokenOrUserID);
        }
        else
        {
            Console.WriteLine("Joining Room {0} with UserID {1} using AccessKey", roomName, tokenOrUserID);
            room = await Bragi.Client.JoinRoom(roomName, tokenOrUserID);
        }
        if (room == null) return null;
        if (!room.CreateMicrophoneMedia(new OdinNative.Core.OdinMediaConfig(48000, 1)))
        {
            Console.WriteLine("Couldn't create MicrophoneMedia for Room {0}. Leaving Room...", roomName);
            await LeaveRoom(roomName);
            return null;
        }
        else
        {
            return new BragiRoom(room, srcp ?? SetRoomConfigParameter.GetDefault(roomName));
        }
    }
    internal void InitializeRoom()
    {
        RegisterRoomEvents();
        RegisterAudioEvents();
    }
    /// <summary>
    /// Registers the Room events for a given room.
    /// </summary>
    /// <param name="room"></param>
    internal void RegisterRoomEvents()
    {
        if (Room == null) return;
        Room.OnConnectionStateChanged += Room_OnConnectionStateChanged;
        Room.OnMediaActiveStateChanged += Room_OnMediaActiveStateChanged;
        Room.OnMediaAdded += Room_OnMediaAdded;
        Room.OnMediaRemoved += Room_OnMediaRemoved;
        Room.OnMessageReceived += Room_OnMessageReceived;
        Room.OnPeerJoined += Room_OnPeerJoined;
        Room.OnPeerLeft += Room_OnPeerLeft;
        Room.OnPeerUserDataChanged += Room_OnPeerUserDataChanged;
        Room.OnRoomUserDataChanged += Room_OnRoomUserDataChanged;
    }
    /// <summary>
    /// Registers the audio events such as microphone capture, settings change
    /// </summary>
    internal void RegisterAudioEvents()
    {
        BragiAudio.InputDataAvailable += Audio_InputDataAvailable;
    }
    /// <summary>
    /// Unregisters the audio events
    /// </summary>
    internal void UnregisterAudioEvents()
    {
        BragiAudio.InputDataAvailable -= Audio_InputDataAvailable;
    }
    private void Audio_InputDataAvailable(object? sender, BragiWaveInEventArgs e)
    {
        if (Room == null) return;
        if (Room.MicrophoneMedia == null) return;
        if (Config.ForceP2T)
        {
            if (!KeyInput.IsKeyPressed(Config.P2TKey)) return;
        }
        else
        {
            if (BragiAudio.UseP2T && !KeyInput.IsKeyPressed(BragiAudio.P2TKey)) return;
        }
        Room.MicrophoneMedia.AudioPushDataAsync(e.Buffer);
    }
    private void Room_OnRoomUserDataChanged(object sender, RoomUserDataChangedEventArgs e)
    {
        if (Room == null) return;
        BragiEvent.BroadcastEvent("RoomUserDataChanged", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["RoomName"] = e.RoomName!,
                ["Data"] = e.Data!
            }
        });
    }
    private void Room_OnPeerUserDataChanged(object sender, PeerUserDataChangedEventArgs e)
    {
        if (Room == null) return;
        BragiEvent.BroadcastEvent("PeerUserDataChanged", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["PeerId"] = e.PeerId,
                ["UserData"] = e.UserData!
            }
        });
    }
    private void Room_OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (Room == null) return;
        /// TODO Wouldn't it be better to convert the data to json string?
        BragiEvent.BroadcastEvent("MessageReceived", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["PeerId"] = e.PeerId,
                ["Data"] = e.Data!
            }
        });
    }
    private void Room_OnPeerLeft(object sender, PeerLeftEventArgs e)
    {
        if (Room == null) return;
        BragiEvent.BroadcastEvent("PeerLeft", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["PeerId"] = e.PeerId
            }
        });
    }
    private void Room_OnPeerJoined(object sender, PeerJoinedEventArgs e)
    {
        if (Room == null) return;
        BragiEvent.BroadcastEvent("PeerJoined", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["PeerId"] = e.PeerId,
                ["UserId"] = e.UserId!
            }
        });
    }
    private void Room_OnMediaRemoved(object sender, MediaRemovedEventArgs e)
    {
        if (Room == null) return;
        Console.WriteLine("Media was removed in Room {0} from Peer {1}", Room.RoomId, e.MediaStreamId);
        /* TODO is this even necessary and good?
         * I think here should the wave out stop
        BragiEvent.BroadcastEvent("MediaRemoved", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["MediaStreamId"] = e.MediaStreamId
            }
        });
        */
    }
    private void Room_OnMediaAdded(object sender, MediaAddedEventArgs e)
    {
        if (Room == null) return;
        Console.WriteLine("Media was added in Room {0} from Peer {1}", Room.RoomId, e.PeerId);
        /* TODO is this even necessary and good?
         * I think here should the wave out of NAudio start with maybe some custom roomconfig
        BragiEvent.BroadcastEvent("MediaAdded", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["PeerId"] = e.PeerId
            }
        });
        */
    }
    private void Room_OnMediaActiveStateChanged(object sender, MediaActiveStateChangedEventArgs e)
    {
        if (Room == null) return;
        BragiEvent.BroadcastEvent("MediaActiveStateChanged", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["Active"] = e.Active,
                ["MediaStreamId"] = e.MediaStreamId,
                ["PeerId"] = e.PeerId
            }
        });
    }
    private void Room_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
    {
        if (Room == null) return;
        BragiEvent.BroadcastEvent("ConnectionStateChanged", new Dictionary<string, object>()
        {
            ["RoomId"] = Room.RoomId,
            ["Args"] = new Dictionary<string, object>()
            {
                ["ChangeReason"] = e.ChangeReason,
                ["ConnectionState"] = e.ConnectionState,
                ["Retry"] = e.Retry
            }
        });
    }
    /// <summary>
    /// Leaves a room
    /// </summary>
    /// <returns></returns>
    internal async Task<bool> LeaveAndDispose()
    {
        if (Room == null) return false;
        if (Bragi.Client == null) return false;
        if (!await Bragi.Client.LeaveRoom(Room.RoomId)) return false;
        Dispose();
        return true;
    }
    public void Dispose()
    {
        UnregisterAudioEvents();
        if (Room == null) return;
        Collection.Remove(Room.RoomId);
        Room = null;
    }
    /// <summary>
    /// Applies a specific config to the room.
    /// </summary>
    /// <param name="src"></param>
    public void ApplyConfig(SetRoomConfigParameter srcp)
    {
        Config = srcp;
    }
    /// <summary>
    /// Performs a cleanup mechanism on the Room collection
    /// </summary>
    public static void CleanUp()
    {
        if (Bragi.Client != null)
        {
            Bragi.Client.Rooms.FreeAll();
        }
        foreach(KeyValuePair<string, BragiRoom> br in Collection)
        {
            br.Value.Dispose();
        }
        Collection.Clear();
    }
}
