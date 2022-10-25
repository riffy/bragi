# Audio
The commands listed below are related to the audio.

## 0 - GetAudioDevices
Command: `GetAudioDevices`

Bragi Initialization Required: `No`

Description:
Retrieves all audio devices currently connected and active on the local machine.

Parameters: `None`

Response Data:
Type  | Description
------------- | -------------
`object` | Object containing the input and output devices and their respective ID and Name.
```ts
interface GetAudioDevicesResponse {
    In: AudioDevice[];
    Out: AudioDevice[];
}

interface AudioDevice {
    Id: string;
    Name: string;
}
```

Example Message:
```json
{ "UUID": "1234", "Command": "GetAudioDevices" }
```

## 1 - SetAudioSettings
Command: `SetAudioSettings`

Bragi Initialization Required: `Yes`

Updates the current audio settings in total.
If IDs are provided that are not active on the client, an error is returned.

Parameters:
Name | Type | Required ? |Description
------------- | ------------- | ------------- | -------------
`InDeviceId` | `string` | Yes | The ID that shall be used for the audio input device (e.g. microphone)
`OutDeviceId` | `string` | Yes | The ID that shall be used for the audio output device (e.g. speaker)
`Volume` | `number` | No | The volume to use for the output. Default: `100`
`PushToTalkKey` | `number` | No | The general push-to-talk [keycode](https://www.toptal.com/developers/keycode). Can also be set on a per-room basis. If missing or set to `0` (None), push-to-talk is deactivated Default: `0`

```ts
interface AudioSettings {
    InDeviceId: string;
    OutDeviceId: string;
    Volume: number;
    PushToTalkKey: number;
}
```

Response Data: 
- On Success: `None`

Error Codes:
Error Code | Description
------------- | -------------
`0` | Bragi initializion missing
`1` | No input device found for the given `InputDeviceId`
`2` | No output device found for the given `OutputDeviceId`
