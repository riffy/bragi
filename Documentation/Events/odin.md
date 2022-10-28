# ODIN
The [ODIN Events](/Documentation/Events/odin.md) listed below are directly caused by ODIN and are forwarded as broadcast messages. Please consult the [ODIN Manual](https://www.4players.io/odin/sdk/unity/). The typescript interfaces are based on ODIN version `1.2.1`.

## 0 - ConnectionStateChanged
Property | Info
------------ | -------------
Name | `ConnectionStateChanged`
Data | `RoomId: string, Args: ConnectionStateChangedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/connectionstatechangedeventargs/)

```ts
interface ConnectionStateChangedEventData {
    RoomId: string;
    Args: ConnectionStateChangedEventArgs;
}

interface ConnectionStateChangedEventArgs {
    ConnectionState: OdinRoomConnectionState;
    ChangeReason: OdinRoomConnectionStateChangeReason;
    Retry: number;
}

enum OdinRoomConnectionState {
    Connecting = 0,
    Connected = 1,
    Disconnected = 2
}

enum OdinRoomConnectionStateChangeReason {
    ClientRequested = 0,
    ServerRequested = 1,
    ConnectionLost = 2
}
```

Example Broadcast:
```json
{ "Event": "ConnectionStateChanged", "Data": {"RoomId": "TestRoom", "Args": {"ConnectionState": 1, "ChangeReason": 0, "Retry": 0}}}
```

## 1 - MediaActiveStateChanged
Property | Info
------------ | -------------
Event | `MediaActiveStateChanged`
Data | `RoomId: string, Args: MediaActiveStateChangedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/mediaactivestatechangedeventargs/)

```ts
interface MediaActiveStateChangedEventData {
    RoomId: string;
    Args: MediaActiveStateChangedEventArgs;
}

interface MediaActiveStateChangedEventArgs {
    Active: boolean;
    MediaStreamId: number;
    PeerId: number;
}
```

Example Broadcast:
```json
{ "Event": "MediaActiveStateChanged", "Data": {"RoomId": "TestRoom", "Args": {"Active": true, "MediaStreamId": 1337, "PeerId": 123}}}
```

## 2 - MediaAdded
⚠ 
> This Event is currently not broadcasted as it is being worked upon (playing audio).
> The need for this event is also questionable at best.


## 3 - MediaRemoved
⚠ 
> This Event is currently not broadcasted as it is being worked upon (playing audio).
> The need for this event is also questionable at best.


## 4 - PeerJoined
Property | Info
------------ | -------------
Event | `PeerJoined`
Data | `RoomId: string, Args: PeerJoinedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/peerjoinedeventargs/)

```ts
interface PeerJoinedEventData {
    RoomId: string;
    Args: PeerJoinedEventArgs;
}

interface PeerJoinedEventArgs {
    PeerId: number;
    UserId: string;
}
```

Example Broadcast:
```json
{ "Event": "PeerJoined", "Data": {"RoomId": "TestRoom", "Args": {"PeerId": 123, "UserId": "PiepMatz"}}}
```

## 5 - PeerLeft
Property | Info
------------ | -------------
Event | `PeerJoined`
Data | `RoomId: string, Args: PeerLeftEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/peerlefteventargs/)

```ts
interface PeerLeftEventData {
    RoomId: string;
    Args: PeerLeftEventArgs;
}

interface PeerLeftEventArgs {
    PeerId: number;
}
```

Example Broadcast:
```json
{ "Event": "PeerJoined", "Data": {"RoomId": "TestRoom", "Args": {"PeerId": 123}}}
```

## 6 - MessageReceived

⚠ Warning: This event is not fully wrapped, there are open TODOs

Property | Info
------------ | -------------
Event | `MessageReceived`
Data | `RoomId: string, Args: MessageReceivedArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/messagereceivedeventargs/)

```ts
interface MessageReceivedData {
    RoomId: string;
    Args: MessageReceivedArgs;
}

interface MessageReceivedArgs {
    PeerId: number;
    Data: any;
}
```

Example Broadcast:
```json
{ "Event": "MessageReceived", "Data": {"RoomId": "TestRoom", "Args": {"PeerId": 123, "Data": ""}}}
```

## 7 - PeerUserDataChanged

⚠ Warning: This event is not fully wrapped, there are open TODOs

Property | Info
------------ | -------------
Event | `PeerUserDataChanged`
Data | `RoomId: string, Args: PeerUserDataChangedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/peeruserdatachangedeventargs/)

```ts
interface PeerUserDataChangedEventData {
    RoomId: string;
    Args: PeerUserDataChangedEventArgs;
}

interface PeerUserDataChangedEventArgs {
    PeerId: number;
    UserData: any;
}
```

Example Broadcast:
```json
{ "Event": "PeerUserDataChanged", "Data": {"RoomId": "TestRoom", "Args": {"PeerId": 123, "Data": ""}}}
```

## 8 - RoomUserDataChanged

⚠ Warning: This event is not fully wrapped, there are open TODOs

Property | Info
------------ | -------------
Event | `RoomUserDataChanged`
Data | `RoomId: string, Args: RoomUserDataChangedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/roomuserdatachangedeventargs/)

```ts
interface RoomUserDataChangedEventData {
    RoomId: string;
    Args: RoomUserDataChangedEventArgs;
}

interface RoomUserDataChangedEventArgs {
    PeerId: number;
    UserData: any;
}
```

Example Broadcast:
```json
{ "Event": "RoomUserDataChanged", "Data": {"RoomId": "TestRoom", "Args": {"PeerId": 123, "Data": ""}}}
```
