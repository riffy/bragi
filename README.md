<p align="center">
    <a href="https://www.4players.io/odin" alt="ODIN">
        <img src="https://img.shields.io/badge/ODIN-v1.2.1-informational?style=for-the-badge" /></a>
    <a href="#" alt="License_MIT">
        <img src="https://img.shields.io/badge/license-MIT-green?style=for-the-badge"/></a>
</p>

# Bragi
Bragi is an application which enables the usage of [ODIN](#whats-odin) via a local websocket interface. Bragi opens a websocket server which can be used by your application.

If possible, try to avoid using Bragi if your application is capable of integrating ODIN on it's own. If your application lacks the privileges and requirements to integrate ODIN, e.g. by being sandboxed, Bragi can help you offering ODIN to your players / clients. This comes with the downside that all clients need to have Bragi running in the background while using your application.

Bragi itself **doesn't** do anything on his own. Everytime the application is started, a complete setup has to be performed, setting the *Instruments* (Audio Devices) etc.

An example how to use Bragi in a project will follow.

## What's ODIN ?
[ODIN](https://www.4players.io/odin) is a cross-platform software development kit (SDK) that enables developers to integrate real-time chat technology into multiplayer games, apps and websites. It is maintained and developed by [4Players](https://github.com/4Players).

# Getting started
## ODIN
Bragi does **not** provide the necessary `odin.dll` that needs to be placed in your release/debug folder. 

You can download it [here](https://github.com/4Players/odin-sdk/releases/) for your specific platform. Make sure that the version matches the supported version in the shield above.

## Using Bragi for your application
You can use any websocket client, e.g. Browser extensions, to connect to Bragi.

Please read documentation.

# What's up with the Norse Mythology ?
It seemed fun to use norse mythology for this project, but don't worry, only the GUI side is affected by this. Here is an explanation if necessary:

* [Bragi](https://en.wikipedia.org/wiki/Bragi) (Application): God of poetry (and music) - is a son of Odin. He greets heroes in Valhalla upon arrival. 
* Valhalla (Websocket Server) 
* Hero (Client): The hero enters valhalla (establishes connection)

_For additional points: `Iðunn` is the wife of Bragi, also a fitting name for the websocket client side in your application_

# Contributing
Since this is my first real C# project, feel free to contribute via pull requests, especially in regards to build or assembly things.

# Credits
We are using code from the following open source projects (in no particular order):
* [ODIN](https://github.com/4Players/odin-sdk-unity)
* [NAudio](https://github.com/naudio/NAudio) - Capturing audio input and playing audio to set devices
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) - Parsing commands received by the websocket client
* [Websocket-Sharp](https://github.com/sta/websocket-sharp) - Communication between applications

## License
⚠ In Progress
