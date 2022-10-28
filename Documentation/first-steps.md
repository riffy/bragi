# First Steps
Bragi does **not** provide the necessary `odin.dll` that needs to be placed in your release/debug folder. 

You can download it [here](https://github.com/4Players/odin-sdk/releases/) for your specific platform. Make sure that the version matches the supported version in the shield above.
## Build from Scratch

**IMPORTANT**: This repo uses [AvalonUI](https://avaloniaui.net/). The extension [AvaloniaVS](https://github.com/AvaloniaUI/AvaloniaVS) is recommend during development.

1. Clone this repo
2. Download the `odin.dll` mentioned above and place it in the `/Bragi` folder alongside `App.axaml.cs`
3. Open the Project with Visual Studio
4. Download the necessary NuGet packages
5. Adjust the properties of the `odin.dll` and set:
    1. `Build Action:` `Content`
    2. `Copy To Output Directory`: `Copy if newer`
6. Run `Debug` or `Release` with `x64` Plattform

Check if everything is working correctly by the following the [next steps](#connecting-to-bragi).

## Connecting to Bragi
Assuming you have Bragi running, you can run the following commands to check if everything is working correctly.

1. Open Bragi and connect to the websocket under `ws://localhost:30159/`. You can use a browser extension like [WebSocket Weasel](https://addons.mozilla.org/de/firefox/addon/websocket-weasel/) for FireFox or something similiar.
    1. On success, you should see `Hero: Arrived` under the `Valhalla` tab and have a corresponding log entry.
2. To test the audio layer you can send the following command: `{"uuid": "1234", "command": "GetAudioDevices"}`
    1. On the websocket you should now get all connected and active audio devices
3. [Initialize](/Documentation/Commands/bragi.md#1---initialize) Bragi with the following command: `{"uuid":"1234","command":"Initialize","params":{"AudioSettings":{"InDeviceId":"$REPLACETHIS","OutDeviceId":"$REPLACETHIS"},"AccessKey":"$REPLACETHIS"}}`
    1. Replace the values for `InDeviceId` and `OutDeviceId` with Ids from the previous message
    2. Replace the access key with a new generated one from [here](https://www.4players.io/odin/introduction/access-keys/)
    3. If the received `Code` is `0`, Bragi is initialized
4. Try to [Join a Room](/Documentation/Commands/odin.md#00---joinroom) with the following command `{"uuid":"1234","command":"JoinRoom","params":{"RoomName":"Test","TokenOrUserId":"PiepMatz"}}`.
    1. You should retrieve information about the room upon a successful join.

You are now good to go :)