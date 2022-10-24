# Bragi
These commands are directly connected to Bragi itself.

## 0 - Ping
Command: `Ping`

Bragi Initialization Required: `No`

Description:
Pings the websocket server and returns a "Pong".

Parameters: `None`

Response Data:
Type  | Description
------------- | -------------
`string` | A simple "Pong"

Example Message:
```json
{ "UUID": "1234", "Command": "Ping" }
```

## 1 - Initialize
Command: `Initialize`

Bragi Initialization Required: `No`

Description:
This is the first command that is expected to be received. It can only be executed **once** per connection. If a re-initialization is required, the client should disconnect first (this triggers an internal cleanup).Initializes an ODIN Client. The `AudioSettings` are considered mandatory to ensure devices are correct etc.

If accesskey is set, the [JoinRoom](/Documentation/Commands/odin.md#00---joinroom) command will be interpreted using the AccessKey.

Parameters:
Name | Type | Required ? | Description
------------- | ------------- | ------------- | -------------
`AudioSettings` | `AudioSettings` | Yes | The [Audio Settings](/Documentation/Commands/audio.md#1---setaudiosettings)
`OdinServer` | `string` | No | The gateway for ODIN to use. Default: Used from shared project
`AccessKey` | `string` | No | Sets the internal accesskey for Odin. **IMPORTANT:** Only set this during development.
```ts
interface InitializeCommand {
    AudioSettings: AudioSettings;
    OdinVersion: string;
    AccessKey: string;
}
```

Response Data:
- On Success: `None`

Error Codes:
Error Code | Description
------------- | -------------
`0` | No input device found for the given `InputDeviceId`
`1` | No input device found for the given `OutputDeviceId`
`2` | Bragi is already initialized

```ts
enum InitializeError {
    INVALID_INPUT_DEVICE,
    INVALID_OUTPUT_DEVICE,
    BRAGI_ALREADY_INITIALIZED
}
```