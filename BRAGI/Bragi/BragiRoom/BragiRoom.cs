using BRAGI.Bragi.Events;
using OdinNative.Odin.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRAGI.Bragi.BragiRoom
{
    public class BragiRoom
    {
        public Room Room { get; private set; }
        public BragiRoom(Room r)
        {
            Room = r;
            RegisterRoomEvents();
        }


        /// <summary>
        /// Leaves a room by the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True on success, False on fail</returns>
        public static async Task<bool> LeaveRoom(string name)
        {
            if (Bragi.Client == null) return false;
            return await Bragi.Client.LeaveRoom(name);
        }

        /// <summary>
        /// Joins a room by the provided information. If the access key is set, the second parameter is interpreted as the token.
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="tokenOrUserID"></param>
        /// <returns></returns>
        public static async Task<BragiRoom?> JoinRoom(string roomName, string tokenOrUserID)
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
            if (room != null) return new BragiRoom(room);
            return null;
        }

        /// <summary>
        /// Registers the Room events for a given room.
        /// </summary>
        /// <param name="room"></param>
        internal void RegisterRoomEvents()
        {
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

        private void Room_OnRoomUserDataChanged(object sender, RoomUserDataChangedEventArgs e)
        {
            BragiEvent.BroadcastEvent("RoomUserDataChanged", new Dictionary<string, object>()
            {
                ["RoomId"] = Room.RoomId,
                ["Args"] = new Dictionary<string, object>()
                {
                    ["RoomName"] = e.RoomName,
                    ["Data"] = e.Data
                }
            });
        }

        private void Room_OnPeerUserDataChanged(object sender, PeerUserDataChangedEventArgs e)
        {
            BragiEvent.BroadcastEvent("PeerUserDataChanged", new Dictionary<string, object>()
            {
                ["RoomId"] = Room.RoomId,
                ["Args"] = new Dictionary<string, object>()
                {
                    ["PeerId"] = e.PeerId,
                    ["UserData"] = e.UserData
                }
            });
        }

        private void Room_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            /// TODO Wouldn't it be better to convert the data to json string?
            BragiEvent.BroadcastEvent("MessageReceived", new Dictionary<string, object>()
            {
                ["RoomId"] = Room.RoomId,
                ["Args"] = new Dictionary<string, object>()
                {
                    ["PeerId"] = e.PeerId,
                    ["Data"] = e.Data
                }
            });
        }

        private void Room_OnPeerLeft(object sender, PeerLeftEventArgs e)
        {
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
            BragiEvent.BroadcastEvent("PeerJoined", new Dictionary<string, object>()
            {
                ["RoomId"] = Room.RoomId,
                ["Args"] = new Dictionary<string, object>()
                {
                    ["PeerId"] = e.PeerId,
                    ["UserId"] = e.UserId
                }
            });
        }

        private void Room_OnMediaRemoved(object sender, MediaRemovedEventArgs e)
        {
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
    }
}
