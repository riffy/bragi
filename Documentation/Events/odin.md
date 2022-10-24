# ODIN
The [ODIN Events](/Documentation/Events/odin.md) listed below are directly caused by ODIN and are forwarded as broadcast messages. Please consult the [ODIN Manual](https://www.4players.io/odin/sdk/unity/). The typescript interfaces are based on ODIN version `1.2.1`.

```
Maybe remove this
Some events may have data stripped from, e.g. the media of [MediaAddedEvent](#1---mediaaddedevent) and encoding/buffer from any [UserData](https://www.4players.io/odin/sdk/unity/classes/userdata/).
```

## 0 - ConnectionStateChanged
Property | Info
------------ | -------------
Name | `ConnectionStateChanged`
Data | `ConnectionStateChangedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/connectionstatechangedeventargs/)

```ts
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

## 1 - MediaActiveStateChanged
Property | Info
------------ | -------------
Event | `MediaActiveStateChanged`
Data | `MediaActiveStateChangedEventArgs`
Link | [Manual](https://www.4players.io/odin/sdk/unity/classes/mediaactivestatechangedeventargs/)

```ts
interface MediaActiveStateChangedEventArgs {
    MediaStreamId: number;
    PeerId: number;
    Active: boolean;
}
```

## 2

## 1 - MediaAddedEvent
⚠ Removed:
* Media Data
* Encoding from UserData
* Buffer from UserData

Property | Info / Link
------------ | -------------
Event | `MediaAddedEvent`
Data | See [MediaAddedEventArgs](https://www.4players.io/odin/sdk/unity/classes/mediaaddedeventargs/)

```ts
interface MediaAddedEventArgs {
    PeerId: number;
    Peer: Peer;
}

interface Peer {
    Id: number;
    RoomName: string;
    UserId: string | null;
    UserData: {};
}
```