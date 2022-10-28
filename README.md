<p align="center">
    <a href="https://www.4players.io/odin" alt="ODIN">
        <img src="https://img.shields.io/badge/ODIN-v1.2.1-informational?style=for-the-badge" /></a>
    <a href="#" alt="License_MIT">
        <img src="https://img.shields.io/badge/license-MIT-green?style=for-the-badge"/></a>
</p>

# BRAGI
BRAGI is an application which enables the usage of [ODIN](#whats-odin) via a local websocket interface. BRAGI opens a websocket server which can be used by your application.

If possible, try to avoid using BRAGI if your application is capable of integrating ODIN on it's own. If your application lacks the privileges and requirements to integrate ODIN, e.g. by being sandboxed, BRAGI can help you offering ODIN to your players / clients. This comes with the downside that all clients need to have BRAGI running in the background while using your application.

BRAGI itself **doesn't** do anything on his own. Everytime the application is started, a complete setup has to be performed, setting the *Instruments* (Audio Devices) etc.

An example how to use BRAGI in a project will follow.

## What's ODIN ?
[ODIN](https://www.4players.io/odin) is a cross-platform software development kit (SDK) that enables developers to integrate real-time chat technology into multiplayer games, apps and websites. It is maintained and developed by [4Players](https://github.com/4Players). This project is based using the Unity SDK of ODIN.

# Getting started
## Developing BRAGI
If you want to help / improve BRAGI, please read the [First Steps](/Documentation/first-steps.md). If you encounter an error, feel free to open an issue.

## Using BRAGI for your application
You can use any websocket client, e.g. Browser extensions, to connect to BRAGI.

Please read documentation.

# What's up with the Norse Mythology ?
It seemed fun to use norse mythology for this project, but don't worry, only the GUI side is affected by this. Here is an explanation if necessary:

* [BRAGI](https://en.wikipedia.org/wiki/BRAGI) (Application): God of poetry (and music) - is a son of Odin. He greets heroes in Valhalla upon arrival. 
* Valhalla (Websocket Server) 
* Hero (Client): The hero enters valhalla (establishes connection)

_For additional points: `Iðunn` is the wife of BRAGI, also a fitting name for the websocket client side in your application_

# Roadmap / TODOs
## General

Basics:![](https://geps.dev/progress/100)
Name | Progress | Description
------------- | ------------- | -------------
`Submoduling` | ![](https://geps.dev/progress/0) | (Halted) Make the `odin-wrapper-csharp` a submodule from [here](https://github.com/4Players/odin-sdk-unity/tree/master/Runtime/OdinWrapper) or if there is a common C# SDK.

## ODIN

Basics: ![](https://geps.dev/progress/50)
Name | Progress | Description
------------- | ------------- | -------------
`Event Broadcast` | ![](https://geps.dev/progress/85) | Broadcast all ODIN Events to the websocket
`Room Methods` | ![](https://geps.dev/progress/25) | Wrap room methods in commands
`Peer Methods` | ![](https://geps.dev/progress/10) | Wrap peer methods in commands 

## Audio

Basics: ![](https://geps.dev/progress/1)
Name | Progress | Description
------------- | ------------- | -------------
`Event Broadcast` | ![](https://geps.dev/progress/0) | Broadcast audio events to the websocket
`Play Audio` | ![](https://geps.dev/progress/0) | Play audio based on received MediaStream from ODIN
`Spatial Audio` | ![](https://geps.dev/progress/0) | Integrate a simplified spatial audio feature based on positions of peers 
`Record Audio` | ![](https://geps.dev/progress/0) | Integrate microphone reader and write to the respective ODIN rooms
`Push-To-Talk` | ![](https://geps.dev/progress/0) | Support Push-To-Talk 
`Push-To-Talk Enhancement` | ![](https://geps.dev/progress/0) | Support Push-To-Talk on a per-room-basis

# Contributing
Since this is my first real C# project, feel free to contribute via pull requests, especially in regards to build or assembly things.

## Contribution Guidline
When doing a commit or pull request a similiar convention like in [Karma](http://karma-runner.github.io/6.3/dev/git-commit-msg.html) is used:

```
<type>: <scope>: <message>

<Description>
```

Type could be `tech`, `fix`, `feat`, `chore`, `docs`, ...
Scope could be `BRAGI`, `ODIN`, `Util`...

Example Commit Message:
```
tech: ODIN: Update wrapper version to 1.2.1

Updated the used version in the odin-wrapper-csharp to the newest version. No changes required
```


# Credits
We are using code or libraries from the following open source projects (in no particular order):
* [ODIN Unity SDK](https://github.com/4Players/odin-sdk-unity)
* [NAudio](https://github.com/naudio/NAudio) - Capturing audio input and playing audio to set devices
* [Websocket-Sharp](https://github.com/sta/websocket-sharp) - Communication between applications
* [AvalonUI](https://avaloniaui.net/)
