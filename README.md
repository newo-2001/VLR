# VLR
A background application that monitors and restores network connectivity whilst streaming games over SteamLink.

## Background
Whilst streaming games over SteamLink my pc would randomly be disconnected from the local network, causing the stream to die.
This background application continuously monitors the connectivity and immediately resets the network adapter if a connection drop is detected; restoring connectivity before SteamLink times out.
It also allows for other code to hook into this event to perform actions depending on the game in question (like pausing the game if a connection drop is detected.)

# Setup
Note: The project only supports Windows.

Before building the application, run `setup.ps1` to generate C# bindings for some win32 classes.