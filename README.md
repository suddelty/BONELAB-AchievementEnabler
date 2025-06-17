# BONELAB Achievement Enabler

[![GitHub Downloads](https://img.shields.io/github/downloads/suddelty/BONELAB-AchievementEnabler/total?style=flat-square)](https://github.com/suddelty/BONELAB-AchievementEnabler/releases)

A MelonLoader mod for BONELAB that enables achievements when using mods.

## Overview

This mod patches the game's achievement system to prevent mods from disabling Steam achievements. It works by:

- Detecting and patching achievement validation methods
- Overriding mod detection systems that might block achievements  
- Continuously ensuring achievement systems remain enabled during gameplay
- Using Harmony patching to intercept and modify achievement-related code

## Quick Links
- [Features](#features)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Install](#install)
- [Usage](#usage)
  - [Console Output](#console-output)
- [Troubleshooting](#troubleshooting)
  - [Common Issues](#common-issues)
  - [Debug Mode](#debug-mode)
- [Compatibility](#compatibility)
  - [Tested With](#tested-with)
  - [Known Incompatibilities](#known-incompatibilities)
- [Development](#development)
  - [Building from Source](#building-from-source)
  - [Build Output Directories](#build-output-directories)
  - [Project Structure](#project-structure)
- [Technical Details](#technical-details)
  - [How It Works](#how-it-works)
  - [Harmony Patches](#harmony-patches)
- [License](#license)
- [Disclaimer](#disclaimer)
- [Changelog](#changelog)

## Features

- **Universal Compatibility**: Works with any combination of BONELAB mods
- **Real-time Protection**: Continuously monitors and maintains achievement functionality
- **Automatic Detection**: Automatically finds and patches achievement-related systems
- **Minimal Performance Impact**: Lightweight implementation with efficient patching
- **Extensive Logging**: Detailed console output for troubleshooting

## Installation

### Prerequisites

1. **BONELAB Patch #6** - The game must be installed and working
2. **MelonLoader 0.6.1+** - Download from [MelonLoader Releases](https://github.com/LavaGang/MelonLoader/releases)
3. **.NET 6.0 Runtime** - Usually installed automatically with MelonLoader

### Install

1. Download the latest release from the [Releases](https://github.com/suddelty/BONELAB-AchievementEnabler/releases) page
2. Extract `AchievementEnabler.dll` to your BONELAB `Mods` folder
   - Default location: `Steam\steamapps\common\BONELAB\Mods\`
3. Launch BONELAB - the mod will load automatically

## Usage

1. Install the mod following the instructions above
2. Launch BONELAB with any other mods you want to use
3. Play the game normally - achievements will work as expected
4. Check the MelonLoader console for confirmation that patches were applied

### Console Output

When working correctly, you should see messages like:
```
[Achievement Enabler] Achievement Enabler Mod Starting...
[Achievement Enabler] Found potential achievement type: SteamAchievementManager
[Achievement Enabler] Patched method: SteamStats.CanUnlockAchievement
[Achievement Enabler] Achievement Enabler patches applied successfully!
```

## Troubleshooting

### Achievements Still Not Working?

1. **Verify MelonLoader Installation**: Ensure MelonLoader is properly installed and working
2. **Check Console Output**: Look for error messages in the MelonLoader console
3. **Game Updates**: After BONELAB updates, achievement systems may change and require mod updates
4. **Steam Overlay**: Ensure Steam overlay is enabled for BONELAB
5. **Steam Login**: Make sure you're logged into Steam

### Common Issues

- **"No bootstrapper found"**: Your MelonLoader installation is corrupted, reinstall MelonLoader
- **Mod not loading**: Ensure the DLL is in the correct `Mods` folder, not `Plugins`
- **Compilation errors**: Make sure you have the correct GamePath set and all dependencies are available

### Debug Mode

For detailed troubleshooting, enable MelonLoader debug mode:
1. Add `--melonloader.debug` to your BONELAB launch options in Steam
2. Launch the game and check the detailed console output
3. Report any errors in the Issues section

## Compatibility

### Tested With
- MelonLoader 0.6.1+
- BONELAB Patch #6

### Known Incompatibilities
- None currently known

## Development

### Building from Source

1. Clone the repository
2. Run `build.bat`
3. The DLL will be built to the `builds/net6.0` folder
4. Optionally:
   - Set the `GamePath` environment variable to your BONELAB installation before building
   - Choose to copy the mod directly to your game's Mods folder when prompted

### Build Output Directories
These directories appear after building the mod yourself:
- `builds/` - Contains the compiled mod DLL and dependencies
- `obj/` - Temporary build files and intermediate compilation outputs (can be safely deleted)

### Project Structure
- `AchievementEnabler.cs` - Main mod code with Harmony patches
- `AchievementEnabler.csproj` - Project file with dependencies

## Technical Details

### How It Works

The mod uses several strategies to ensure achievements remain enabled:

1. **Method Patching**: Uses Harmony to patch methods that check for mods or validate achievements
2. **Type Discovery**: Automatically finds achievement-related classes in all loaded assemblies  
3. **Runtime Monitoring**: Continuously checks and fixes achievement state during gameplay
4. **Mod Detection Override**: Patches mod detection methods to return false

### Harmony Patches

- `AllowAchievementPrefix`: Bypasses achievement validation methods
- `NoModsDetectedPrefix`: Makes mod detection methods return false
- Runtime field patching for achievement manager instances

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Disclaimer

This mod is provided as-is for educational and convenience purposes. Use at your own risk. The developers are not responsible for any issues that may arise from using this mod.

## Changelog

### v1.0.0
- Initial release
- Automatic achievement system detection and patching
- Real-time achievement state monitoring
- Comprehensive mod detection override
- Full MelonLoader integration 