# ODIN Commands
The commands listed below are directly related to ODIN. Please consult the [ODIN Manual](https://www.4players.io/odin/sdk/unity/). The typescript interfaces are based on ODIN version `1.2.1`.

## 0 - Room(s)

### 0.0 - JoinRoom
Command: `JoinRoom`

Bragi Initialization Required: `Yes`

Description:
Initiates the ODIN client to join a specified room by calling `OdinClient.JoinNamedRoom` using the token. If the access key was previously set during the initialization, it will instead call `OdinClient.JoinRoom` with the parameter as the `UserId` (⚠ only use this during development)

Parameters:
Name | Type | Required ? | Description
------------- | ------------- | ------------- | -------------
`RoomName` | `string` | Yes | The Room to join.
`TokenOrUserId` | `string` | Yes | The Token used for joining the room. If the access key is set, it is interpreted as the UserId
```ts
interface JoinRoomCommand {
    RoomName: string;
    TokenOrUserId: string;
}
```

Response Data: 
Data | Type | Description
------------- | ------------- | -------------
`Id` | `string` | The Id or name of the room that was joined
`Peers` | `number[]` | An array of all other peers in that room (excludes self)
`Self` | `object{}` | A simplified peer object of one self

```ts
interface SimplifiedPeer {
    Id: number;
    RoomName: string;
    UserId: string | null;
}

interface JoinRoomResponse {
    Id: string;
    Peers: number[];
    Self: SimplifiedPeer;
}
```

Error Codes:
Error Code | Description
------------- | -------------
`0` | Bragi initializion missing
`1` | Room could not be joined (Room was `null`)

```ts
enum JoinRoomError {
    BRAGI_NOT_INITIALIZED,
    ROOM_JOIN_FAILURE
}
```

Example Message:
```json
{"uuid":"1234","command":"JoinRoom","params":{"RoomName":"TestRoom","TokenOrUserId":"PiepMatz"}}
```


### 0.1 - LeaveRoom
Command: `LeaveRoom`

Bragi Initialization Required: `Yes`

Description:
Leaves a certain Room by the provided Name

Parameters:
Name | Type | Required ? | Description
------------- | ------------- | ------------- | -------------
`RoomName` | `string` | Yes | The Room to leave
```ts
interface LeaveRoomCommand {
    RoomName: string;
}
```

Response Data: 
- On Success: `boolean`
    - `true` - Room was successfully left
    - `false` - Room could not be left

Error Codes:
Error Code | Description
------------- | -------------
`0` | Bragi initializion missing

```ts
enum LeaveRoomError {
    BRAGI_NOT_INITIALIZED
}
```

Example Message:
```json
{"uuid":"1234","command":"LeaveRoom","params":{"RoomName":"TestRoom"}}
```
