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


### Incompatibilities
- Unfortunately incompatible with Refuel Waypoint since both mods use a Harmony prefix patch on SetWaypoint, but Waypoint Queue has built-in support for refueling locomotives similar to Refuel Waypoint.

## How to use the mod
Waypoint Queue significantly expands the Auto Engineer (AE) Waypoint mode feature from vanilla Railroader.

All the keybinds are reconfigurable using the UMM Mod Settings menu which you can access by pressing Ctrl+F10.

### Key concepts

- **Queue a waypoint** by holding Ctrl while selecting a waypoint.
- Placing a normal waypoint without Ctrl pressed will clear the locomotive's waypoint queue.
- **Manage waypoints** for the currently selected locomotive by opening the Waypoints window with Shift+G.
- The Waypoints window has many advanced features to handle complex orders like switching.
- **Jump to waypoint** by clicking the gear icon for that waypoint in the new window.
- **Replace the current waypoint** without clearing the rest of the queue by holding Alt instead of Ctrl.
- **Delete a waypoint** by clicking the gear icon, or delete all waypoints for the sel

### Coupling and uncoupling
- **Uncouple cars at a waypoint** by adjusting the number of cars to uncouple and from which direction to count cars.
- **Handle air and handbrakes** when coupling or uncoupling.
    - When coupling, you can order the engineer to connect air and release handbrakes on the coupled cars (enabled by default)
    - When uncoupling, you can order the engineer to set handbrakes and bleed air cylinders on the uncoupled cars (enabled by default)

### Refueling
- **Refuel locomotives** by selecting a waypoint close to a valid water tower, coaling station, or diesel pump.
- Currently, the loaders will not animate, but they will transfer loads from storage to the locomotive correctly.

### Advanced features
- **Take or leave** a number of cars after coupling. This is very useful when queueing switching orders.
    - **Take** - If you couple to a cut of 3 cars and *Take* 2 cars, you will leave with the 2 closest cars and the 3rd car will be left behind.
    - **Leave** - If you are coupling 2 additional cars to 1 car already spotted, you can *Leave* 2 cars and continue to the next queued waypoint.
- **Take active cut** when uncoupling to help when road switching (disabled by default)
    - When **Take active cut** is selected, the number of cars to uncouple will still be part of the active train. The rest of the train will be treated as an uncoupled cut which may bleed air and apply handbrakes. This is particularly useful for local freight switching.
    - A train of 10 cars arrives in Whittier. The 2 cars behind the locomotive need to be delivered. By checking **Take active cut**, you can order the engineer to travel to a waypoint, uncouple 4 cars including the locomotive and tender, and travel to another waypoint to the industry track to deliver the 2 cars, all while knowing that the rest of the local freight consist has handbrakes applied.

## Local Development

### Requirements
- Visual Studio (in the installer, make sure you have SDKs for .NET Framework 4.8, C# support, and NuGet Package Manager)

To start developing locally, follow these steps:
1. Clone the repo locally
2. Copy `Paths.user.example` and save it as `Paths.user`. Open this `Paths.user` file and update the path to the game directory containing `Railroader.exe`
3. Open the Solution

### During Development
Make sure you're using the *Debug* configuration. Every time you build your project, the files will be copied to your Mods folder and you can immediately start the game to test it.