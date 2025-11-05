# Waypoint Queue

## Installation

### Requirements
- [Unity Mod Manager](https://www.nexusmods.com/site/mods/21)

### Steps:
1. Download the latest release
2. Open UMM Installer and make sure you have Railroader selected
3. Click the Mods tab
4. Drag and drop the `WaypointQueue.zip` file into UMM
5. Start up the game

## Local Development

### Requirements
- Visual Studio (in the installer, make sure you have SDKs for .NET Framework 4.8, C# support, and NuGet Package Manager)

To start developing locally, follow these steps:
1. Clone the repo locally
2. Copy `Paths.user.example` and save it as `Paths.user`. Open this `Paths.user` file and update the path to the game directory containing `Railroader.exe`
3. Open the Solution

### During Development
Make sure you're using the *Debug* configuration. Every time you build your project, the files will be copied to your Mods folder and you can immediately start the game to test it.