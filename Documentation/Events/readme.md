# Events
Bragi broadcasts events in a websocket message in a JSON format to be consumed by your client application.
Please always refer to the manual link of the ODIN event under each section for more details.

Audio and additional events are also broadcasted.

## Syntax
Property | Type | Description
------------ | ------------- | -------------
Event | `string` | Name of the event that occured
Data | `object{}` | Additional data, such as parameters / arguments that describe the event that occured. Is **always** present, but can be empty.

The following example message is broadcasted on the websocket, when the client joins a room. Read [ConnectionStateChanged](/Documentation/Events/odin.md#0---connectionstatechanged) for more info
Example Broadcast:
```json
{ "Event": "ConnectionStateChanged", "Data": {"ConnectionState": 1, "ChangeReason": 0, "Retry": 0} }
```