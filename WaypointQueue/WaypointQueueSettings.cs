using UnityEngine;
using UnityModManagerNet;

namespace WaypointQueue
{
    public class WaypointQueueSettings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Queued Waypoint mode keybinding")]public KeyBinding queuedWaypointModeKey = new KeyBinding() { keyCode = KeyCode.LeftControl };

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }
}
