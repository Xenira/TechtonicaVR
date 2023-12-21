# Techtonica VR Mod

![GitHub release (with filter)](https://img.shields.io/github/v/release/Xenira/TechtonicaVR)
![GitHub Release Date - Published_At](https://img.shields.io/github/release-date/Xenira/TechtonicaVR)
![GitHub downloads all releases](https://img.shields.io/github/downloads/Xenira/TechtonicaVR/total)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Xenira/TechtonicaVR/dotnet.yml)
![GitHub issues](https://img.shields.io/github/issues/Xenira/TechtonicaVR)
![GitHub pull requests](https://img.shields.io/github/issues-pr/Xenira/TechtonicaVR)
![License](https://img.shields.io/github/license/Xenira/TechtonicaVR)

<p align="center">
  <img src="icon.png" width="256" />
</p>

This is a mod for the game [Techtonica](https://store.steampowered.com/app/1457320/Techtonica/) that adds VR support.

**🔥 CAUTION**\
This mod is still in early development and **will** contain bugs. Use at your own risk. Only tested with the Valve Index / Pimax Crystal and Knuckles controllers.

## Prerequisites

* Version 0.2.0f of the game. As the game is still in early access, this mod may not work with future versions of the game. If you encounter any issues, please create an [Issue](https://github.com/Xenira/TechtonicaVR/issues).
* [BepInEx](https://github.com/BepInEx/BepInEx) version BepInEx Version

## Setup

### BepInEx
1. Download the latest 5.x release of BepInEx from the [Releases](https://github.com/BepInEx/BepInEx/releases) page.
2. Extract the downloaded archive into the game’s installation directory.
3. Run the game once. BepInEx should be installed automatically.

### VR Mod
1. Download the latest release of this mod from the [Releases](https://github.com/Xenira/TechtonicaVR/releases) page.
2. Extract the downloaded archive.
3. Copy the `BepInEx` and `Techtonica_Data` folders to the `Techtonica` folder in `steamapps/common` directory.
4. Run the game. The mod should be loaded automatically by BepInEx.

### Disabling the Mod
To disable the mod change the `Enabled` under `[General]` value in the `de.xenira.techtonica.cfg` file in the `BepInEx/config` folder to `false`.

### Uninstalling the Mod
To uninstall the mod remove the mod’s files from the `BepInEx/plugins` folder. This should be the following files:

* `techtonica_vr.dll`
* 'techtonica_vr' folder

There will be some leftover files in the `Techtonica_Data` folder. To clean those up, delete the folder and verify the game’s files in Steam. This will redownload the game’s files and remove any leftover files.

## Current State
While the mod is in a playable state, it is still in early development. Some features are still missing and there are some known issues. Other things might be a little yanky or not work as expected.

### Working
* Rendering of the game world with 6DOF Tracking
* Tracking of the player’s head and hands
* Smooth locomotion and turning
* Comfort options (Teleportation, Snap turning, Vignette)
* Basic controller bindings (Although they might not be optimal)
* Controller haptics
* Smooth turning
* UI

### Missing
* Gesture support (e.g. Mining using pickaxe motion)
* Model for the player’s body. Currently not a priority as this requires IK.
* Default bindings for VR controllers other than the Valve Index Controllers (#16, #17)
* Object outlines. Disabled for now as the shader is broken in VR.
* Finger tracking (#15)
* Ability to switch primary hand
* Ability to yeet paladin down the waterfall
* Hand crank using uhhhhh... hands?

### Known Issues
* Button prompts are not for VR controllers. (#13)
* Haptics are played on both controllers by the game. One improvement would be to play them on the hand that is actually holding the tool.
* The game is locked to 60fps when running in windowed mode. This is based on the refresh rate of your monitor. To unlock the framerate, switch to fullscreen mode. (#10)
* Menus are a little jittery (#18)

### Cool stuff to try
* Tobii eye tracking for dynamic foveated rendering
* Enable ik (The game ships with `FinalIK` so it should be possible. Probably just not networked yet.)

## Troubleshooting

If you encounter any issues while using this mod, please check the BepInEx console for any error messages. You can also report issues on the [Issues](https://github.com/Xenira/TechtonicaVR/issues) page of this repository.

### Q&A
1. _Why is my framerate locked to 60fps?_\
The game is locked to x fps when running in Windowed mode. This is based on the refresh rate of your monitor. To unlock the framerate, switch to fullscreen mode. (For now)
2. _Does this mod work with Gamepass?_\
Yes, the mod needs to be installed in `Gamepass/Techtonica/Content`.
3. _Why is the mod not open source?_\
It is. You are looking at the source right now (duh!).
4. _Why is the mod not on NexusMods?_\
I don’t like NexusMods. I don’t like their ToS and I don’t like their mod manager. I don’t want to support them.

## License
This mod is licensed under the GNU General Public License v3.0 (GPL-3.0).

Contents of the `unity` and `libs` folders are licensed under their respective licenses.

## Disclaimer
This mod is not affiliated with the game’s developers Fire Hose Games, Unity Technologies or Valve Corporation. All trademarks are the property of their respective owners.
