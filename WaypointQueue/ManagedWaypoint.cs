using Model;
using System;
using System.Text.Json.Serialization;
using Track;
using WaypointQueue.UUM;

namespace WaypointQueue
{
    public class ManagedWaypoint
    {
        public enum PostCoupleCutType
        {
            Take,
            Leave
        }
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
                return !IsCoupling && NumberOfCarsToCut > 0;
            }
        }

        public bool ConnectAirOnCouple { get; set; }
        public bool ReleaseHandbrakesOnCouple { get; set; }
        public bool ApplyHandbrakesOnUncouple { get; set; }
        public bool BleedAirOnUncouple { get; set; }

        public int NumberOfCarsToCut { get; set; }
        public bool CountUncoupledFromNearestToWaypoint { get; set; }
        public PostCoupleCutType TakeOrLeaveCut { get; set; }

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

        public ManagedWaypoint(Car locomotive, Location location, string coupleToCarId = "", bool connectAirOnCouple = true, bool releaseHandbrakesOnCouple = true, bool applyHandbrakeOnUncouple = true, int numberOfCarsToCut = 0, bool countUncoupledFromNearestToWaypoint = true, bool bleedAirOnUncouple = true, PostCoupleCutType takeOrLeaveCut = PostCoupleCutType.Take)
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
            NumberOfCarsToCut = numberOfCarsToCut;
            CountUncoupledFromNearestToWaypoint = countUncoupledFromNearestToWaypoint;
            BleedAirOnUncouple = bleedAirOnUncouple;
            TakeOrLeaveCut = takeOrLeaveCut;
        }

        [JsonConstructor]
        public ManagedWaypoint(string id, string locomotiveId, string locationString, string coupleToCarId, bool connectAirOnCouple, bool releaseHandbrakesOnCouple, bool applyHandbrakesOnUncouple, bool bleedAirOnUncouple, int numberOfCarsToCut, bool countUncoupledFromNearestToWaypoint, PostCoupleCutType takeOrLeaveCut)
        {
            Id = id;
            LocomotiveId = locomotiveId;
            LocationString = locationString;
            CoupleToCarId = coupleToCarId;
            ConnectAirOnCouple = connectAirOnCouple;
            ReleaseHandbrakesOnCouple = releaseHandbrakesOnCouple;
            ApplyHandbrakesOnUncouple = applyHandbrakesOnUncouple;
            BleedAirOnUncouple = bleedAirOnUncouple;
            NumberOfCarsToCut = numberOfCarsToCut;
            CountUncoupledFromNearestToWaypoint = countUncoupledFromNearestToWaypoint;
            TakeOrLeaveCut = takeOrLeaveCut;
        }
    }
}
