using Model;
using Model.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Track;
using UI.EngineControls;
using UnityEngine;
using WaypointQueue.UUM;

namespace WaypointQueue
{
    public class WaypointQueueController : MonoBehaviour
    {
        private static WaypointQueueController _shared;

        public static WaypointQueueController Shared
        {
            get
            {
                if (_shared == null)
                {
                    _shared = FindObjectOfType<WaypointQueueController>();
                }
                return _shared;
            }
        }

        public class LocoWaypointQueue
        {
            public Car Locomotive;
            public Queue<MaybeWaypoint> QueuedWaypoints;

            public LocoWaypointQueue(Car loco)
            {
                Locomotive = loco;
                QueuedWaypoints = new Queue<MaybeWaypoint>();
            }
        }

        private List<LocoWaypointQueue> locoWaypointQueues = new List<LocoWaypointQueue>();

        private Coroutine coroutine;

        private IEnumerator Ticker()
        {
            WaitForSeconds t = new WaitForSeconds(0.5f);
            while (true)
            {
                yield return t;
                Tick();
            }
        }

        public void AddLocoWaypoint(Car loco, MaybeWaypoint maybeWaypoint)
        {
            LocoWaypointQueue entry = locoWaypointQueues.Find(x => x.Locomotive.id == loco.id);
            if (entry == null)
            {
                entry = new LocoWaypointQueue(loco);
                locoWaypointQueues.Add(entry);
            }
            entry.QueuedWaypoints.Enqueue(maybeWaypoint);

            if (coroutine == null)
            {
                Loader.LogDebug("Starting coroutine");
                coroutine = StartCoroutine(Ticker());
            }
        }

        public List<MaybeWaypoint> GetWaypointList(Car loco)
        {
            return locoWaypointQueues.Find(x => x.Locomotive.id == loco.id).QueuedWaypoints.ToList();
        }

        private void Tick()
        {
            DoQueueTickUpdate();
        }

        private void Stop()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = null;
        }
        
        private bool IsReadyForNextWaypoint(Car locomotive)
        {
            AutoEngineerOrdersHelper ordersHelper = GetOrdersHelper(locomotive);
            return !ordersHelper.Orders.Waypoint.HasValue;
        }

        private void DoQueueTickUpdate()
        {
            List<LocoWaypointQueue> listForRemoval = new List<LocoWaypointQueue>();

            if (locoWaypointQueues == null) {
                Loader.LogDebug("Stopping coroutine because queue list was null");
                Stop();
            }
            //Loader.LogDebug($"Iterating over the queue list with {locoWaypointQueues.Count} entries");

            foreach (LocoWaypointQueue entry in locoWaypointQueues)
            {
                if (entry.QueuedWaypoints.Count > 0 && IsReadyForNextWaypoint(entry.Locomotive))
                {
                    Queue<MaybeWaypoint> waypointQueue = entry.QueuedWaypoints;
                    MaybeWaypoint maybeWaypoint = waypointQueue.Dequeue();
                    SendToWaypointFromQueue(entry.Locomotive, maybeWaypoint.Location, maybeWaypoint.CoupleToCarId);
                }
                
                if(entry.QueuedWaypoints.Count == 0)
                {
                    //Loader.LogDebug($"Marking entry for removal");
                    listForRemoval.Add(entry);
                }
            }

            locoWaypointQueues = locoWaypointQueues.FindAll(x => !listForRemoval.Contains(x));

            if (locoWaypointQueues.Count == 0)
            {
            Loader.LogDebug("Stopping coroutine because queue list is empty");
                Stop();
            }
        }

        private AutoEngineerOrdersHelper GetOrdersHelper(Car locomotive)
        {
            Type plannerType = typeof(AutoEngineerPlanner);
            FieldInfo fieldInfo = plannerType.GetField("_persistence", BindingFlags.NonPublic | BindingFlags.Instance);
            AutoEngineerPersistence persistence = (AutoEngineerPersistence)fieldInfo.GetValue((locomotive as BaseLocomotive).AutoEngineerPlanner);
            AutoEngineerOrdersHelper ordersHelper = new AutoEngineerOrdersHelper(locomotive, persistence);
            return ordersHelper;
        }

        private void SendToWaypointFromQueue(Car locomotive, Location location, string coupleToCarId)
        {
            Loader.LogDebug($"SendToWaypointFromQueue for {locomotive.id} at {location} and coupling to {coupleToCarId}");
            AutoEngineerOrdersHelper ordersHelper = GetOrdersHelper(locomotive);
            ordersHelper.SetWaypoint(location, coupleToCarId);
        }
    }
}
