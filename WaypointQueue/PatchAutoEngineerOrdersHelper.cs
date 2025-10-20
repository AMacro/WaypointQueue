using UI.EngineControls;
using HarmonyLib;
using Model.AI;
using Track;
using Model;
using WaypointQueue.UUM;
using UnityEngine;

namespace WaypointQueue
{
    [HarmonyPatch(typeof(AutoEngineerOrdersHelper), nameof(AutoEngineerOrdersHelper.SetWaypoint))]
    internal class PatchAutoEngineerOrdersHelper
    {
        public class PatchState
        {
            public bool _isAppendingWaypoint;
            public OrderWaypoint? _oldWaypoint;

            public PatchState(bool isAppendingWaypoint, OrderWaypoint? oldWaypoint)
            {
                _isAppendingWaypoint = isAppendingWaypoint;
                _oldWaypoint = oldWaypoint;
            }
        }

        static bool Prefix(Location location, string coupleToCarId, ref Car ____locomotive, ref AutoEngineerPersistence ____persistence)
        {
            OrderWaypoint? oldWaypoint = ____persistence.Orders.Waypoint;
            bool isAppendingWaypoint = Input.GetKey(Loader.Settings.queuedWaypointModeKey.keyCode);

            if (isAppendingWaypoint && oldWaypoint != null)
            {
                (Location, string)? maybeWaypoint = (location, coupleToCarId);
                if (maybeWaypoint.HasValue)
                {
                    string couplingLogSegment = coupleToCarId != null && coupleToCarId.Length > 0 ? $" and coupling to ${coupleToCarId}" : "";
                    Loader.Log($"Adding waypoint {location}{couplingLogSegment} for loco {____locomotive.id}, after original waypoint {oldWaypoint.Value.LocationString}");
                    WaypointQueueController.Shared.AddLocoWaypoint(____locomotive, new MaybeWaypoint(location, coupleToCarId));
                }
                return false;
            }
            return true;
        }
    }
}
