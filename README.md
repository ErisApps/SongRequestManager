# SongRequestManager
A fairly simple, from the ground up re-implementation of Song Request Manager that uses ChatCore instead of StreamCore.

## Installation
This mod requires a few other mods in order to work.
- BeatSaberMarkupLanguage v1.3.4 or higher
- BeatSaverSharp v1.6.0 or higher
- BSIPA v4.0.5 or higher
- ChatCore v1.0.0 or higher
- SiraUtil v1.0.2 or higher
- SongCore v2.9.11 or higher

Installation is fairly simple.
1. Grab the latest plugin release from the [releases page](https://github.com/ErisApps/SongRequestManager/releases) (once there is a release)
2. Drop the .dll file in the "Plugins" folder of your Beat Saber installation.
3. Boot it up (or reboot)

## Usage
All commands listed below use the prefix `!` by default. This can be  changed in the (in-game) settings however.

### Basic commands
| Command | Alias | Description | Example |
| --- | --- | --- | ---|
| bsr | sr, request | Allows you to request a song using the Beatsaver ID. | bsr abcd


## Developers
To build this project you will need to create a `SongRequestManager/SongRequestManager.csproj.user` file specifying where the game is located:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Change this path if necessary. Make sure it ends with a backslash. -->
    <GameDirPath>D:\Program Files (x86)\Steam\steamapps\common\Beat Saber\</GameDirPath>
  </PropertyGroup>
</Project>
```
