using Track;

namespace WaypointQueue
{
    public class MaybeWaypoint
    {
        public Location Location;
        public string CoupleToCarId;

        public MaybeWaypoint(Location location, string coupleToCarId)
        {
            Location = location;
            CoupleToCarId = coupleToCarId;
        }
    }
}
