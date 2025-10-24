using Model;
using System;
using System.Text.Json.Serialization;
using Track;
using WaypointQueue.UUM;

namespace WaypointQueue
{
    public class ManagedWaypoint
    {
        [JsonInclude]
        public string Id { get; private set; }

        [JsonInclude]
        public string LocomotiveId { get; private set; }

        [JsonIgnore]
        public Car Locomotive { get; private set; }

        [JsonInclude]
        public string LocationString { get; private set; }

        [JsonIgnore]
        public Location Location { get; private set; }

        [JsonInclude]
        public string CoupleToCarId { get; private set; }

        [JsonIgnore]
        public bool IsCoupling
        {
            get
            {
                return CoupleToCarId != null && CoupleToCarId.Length > 0;
            }
        }

        [JsonIgnore]
        public bool IsUncoupling
        {
            get
            {
                return NumberOfCarsToUncouple > 0;
            }
        }

        public bool ConnectAirOnCouple { get; set; }
        public bool ReleaseHandbrakesOnCouple { get; set; }
        public bool ApplyHandbrakesOnUncouple { get; set; }
        public bool BleedAirOnUncouple { get; set; }

        public int NumberOfCarsToUncouple { get; set; }
        public bool UncoupleNearestToWaypoint { get; set; }

        public void Load()
        {
            if (TrainController.Shared.TryGetCarForId(LocomotiveId, out Car locomotive))
            {
                Loader.LogDebug($"Loaded locomotive {locomotive.Ident} for ManagedWaypoint");
                Locomotive = locomotive;
            }
            else
            {
                throw new InvalidOperationException($"Could not find car for {LocomotiveId}");
            }

            Location = Graph.Shared.ResolveLocationString(LocationString);
            Loader.LogDebug($"Loaded location {Location} for {locomotive.Ident} ManagedWaypoint");
        }

        public ManagedWaypoint(Car locomotive, Location location, string coupleToCarId = "", bool connectAirOnCouple = true, bool releaseHandbrakesOnCouple = true, bool applyHandbrakeOnUncouple = true, int numberOfCarsToUncouple = 0, bool uncoupleNearestToWaypoint = true, bool bleedAirOnUncouple = true)
        {
            Id = Guid.NewGuid().ToString();
            Locomotive = locomotive;
            LocomotiveId = locomotive.id;
            Location = location;
            LocationString = Graph.Shared.LocationToString(location);
            CoupleToCarId = coupleToCarId;
            ConnectAirOnCouple = connectAirOnCouple;
            ReleaseHandbrakesOnCouple = releaseHandbrakesOnCouple;
            ApplyHandbrakesOnUncouple = applyHandbrakeOnUncouple;
            NumberOfCarsToUncouple = numberOfCarsToUncouple;
            UncoupleNearestToWaypoint = uncoupleNearestToWaypoint;
            BleedAirOnUncouple = bleedAirOnUncouple;
        }

        [JsonConstructor]
        public ManagedWaypoint(string id, string locomotiveId, string locationString, string coupleToCarId, bool connectAirOnCouple, bool releaseHandbrakesOnCouple, bool applyHandbrakesOnUncouple, bool bleedAirOnUncouple, int numberOfCarsToUncouple, bool uncoupleNearestToWaypoint)
        {
            Id = id;
            LocomotiveId = locomotiveId;
            LocationString = locationString;
            CoupleToCarId = coupleToCarId;
            ConnectAirOnCouple = connectAirOnCouple;
            ReleaseHandbrakesOnCouple = releaseHandbrakesOnCouple;
            ApplyHandbrakesOnUncouple = applyHandbrakesOnUncouple;
            BleedAirOnUncouple = bleedAirOnUncouple;
            NumberOfCarsToUncouple = numberOfCarsToUncouple;
            UncoupleNearestToWaypoint = uncoupleNearestToWaypoint;
        }
    }
}
