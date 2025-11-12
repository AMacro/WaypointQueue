using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Messages;
using HarmonyLib;
using Model.AI;
using UnityEngine;
using WaypointQueue.UUM;

namespace WaypointQueue
{
    [HarmonyPatch(typeof(AutoEngineerPlanner))]
    internal static class Patch_AEPlanner_UpdateTargets_NonStop
    {
        private const string NoStopTag = "[nostop]";

        [HarmonyPostfix]
        [HarmonyPatch("UpdateTargets")]
        static void UpdateTargetsPostfix(
            AutoEngineerPlanner __instance,
            ref AutoEngineer ____engineer
        )
        {
            try
            {
                if (____engineer == null)
                    return;

                var engType = ____engineer.GetType();
                var targetsObj =
                    AccessTools.Field(engType, "_targets")?.GetValue(____engineer) ??
                    AccessTools.Property(engType, "Targets")?.GetValue(____engineer, null);

                if (targetsObj == null)
                    return;

                var tgtType = targetsObj.GetType();

                float plannerMax = 0f;
                var maxField = AccessTools.Field(tgtType, "MaxSpeedMph");
                if (maxField != null)
                {
                    plannerMax = (float)maxField.GetValue(targetsObj);
                    if (Mathf.Abs(plannerMax) > 0.01f)
                    {
                        float sign = Mathf.Sign(plannerMax);
                        plannerMax = plannerMax + 20f * sign;
                        maxField.SetValue(targetsObj, plannerMax);
                    }
                }

                var allTargetsField = AccessTools.Field(tgtType, "AllTargets");
                var allTargets = allTargetsField?.GetValue(targetsObj) as List<AutoEngineer.Targets.Target>;

                if (allTargets != null && allTargets.Count > 0)
                {
                    for (int i = 0; i < allTargets.Count; i++)
                    {
                        var t = allTargets[i];
                        float dir =
                            Mathf.Abs(t.SpeedMph) > 0.01f ? Mathf.Sign(t.SpeedMph) :
                            Mathf.Abs(plannerMax) > 0.01f ? Mathf.Sign(plannerMax) :
                            1f;

                        
                        //if (t.Reason == "Running to waypoint" || t.Reason == "At waypoint")
                        //{
                        //    t.SpeedMph = 10f * dir;
                        //}
                        
                        if (t.Reason == "Track Speed")
                        {
                            
                            float baseSpeed = t.SpeedMph;
                            if (Mathf.Abs(baseSpeed) < 0.01f)
                            {
                                baseSpeed = 0f;
                            }

                            float bump = 10f * dir;
                            t.SpeedMph = baseSpeed + bump;
                        }

                        allTargets[i] = t;
                    }
                }

                
                
                
                var persField = AccessTools.Field(typeof(AutoEngineerPlanner), "_persistence");
                var ordersField = AccessTools.Field(typeof(AutoEngineerPlanner), "_orders");
                var manualStopField = AccessTools.Field(typeof(AutoEngineerPlanner), "_manualStopDistance");

                if (persField != null && ordersField != null)
                {
                    
                    object boxedPersistence = persField.GetValue(__instance);
                    Orders currentOrders = (Orders)ordersField.GetValue(__instance);

                    if (currentOrders.Mode == AutoEngineerMode.Waypoint &&
                        currentOrders.Waypoint.HasValue)
                    {
                        var wp = currentOrders.Waypoint.Value;

                        if (!string.IsNullOrEmpty(wp.LocationString) &&
                            wp.LocationString.IndexOf(NoStopTag, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            var isWpSatMI = AccessTools.Method(
                                typeof(AutoEngineerPlanner),
                                "IsWaypointSatisfied",
                                new[] { typeof(OrderWaypoint) });

                            if (isWpSatMI != null)
                            {
                                bool satisfied = (bool)isWpSatMI.Invoke(__instance, new object[] { wp });
                                if (satisfied)
                                {
                                    var restoreMI = AccessTools.Method(typeof(AutoEngineerPlanner), "RestoreAllRemainingSwitches", Type.EmptyTypes);
                                    var clearRouteMI = AccessTools.Method(typeof(AutoEngineerPlanner), "ClearRoute", Type.EmptyTypes);
                                    var postNoticeMI = AccessTools.Method(typeof(AutoEngineerPlanner), "PostWaypointNotice", new[] { typeof(string) });

                                    restoreMI?.Invoke(__instance, null);
                                    clearRouteMI?.Invoke(__instance, null);
                                    postNoticeMI?.Invoke(__instance, new object[] { "Arrived at waypoint!" });

                                    
                                    manualStopField?.SetValue(__instance, 0f);

                                    
                                    var persistence = (AutoEngineerPersistence)boxedPersistence;
                                    var old = persistence.Orders;
                                    persistence.Orders = new Orders(old.Mode, old.Forward, old.MaxSpeedMph, null);
                                    persField.SetValue(__instance, persistence);

                                    Loader.Log("AE non-stop waypoint: forced satisfaction + cleared waypoint.");
                                }
                            }
                        }
                    }
                }

                
                
                
                DumpTargetsToFile(targetsObj, allTargets, plannerMax);
            }
            catch (Exception ex)
            {
                Loader.Log($"AE non-stop postfix exception: {ex}");
            }
        }

        private static void DumpTargetsToFile(object targetsObj, List<AutoEngineer.Targets.Target> allTargets, float plannerMax)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== AutoEngineerPlanner.UpdateTargets POSTFIX DUMP ===");
                sb.AppendLine($"Time: {DateTime.Now:O}");
                sb.AppendLine($"Planner MaxSpeedMph (after bump): {plannerMax}");
                sb.AppendLine($"Targets type: {targetsObj.GetType().FullName}");

                if (allTargets != null && allTargets.Count > 0)
                {
                    for (int i = 0; i < allTargets.Count; i++)
                    {
                        var t = allTargets[i];
                        sb.AppendLine($"[{i}] speed={t.SpeedMph} distance={t.Distance} reason={t.Reason}");
                    }
                }
                else
                {
                    sb.AppendLine("No target list found or empty.");
                }

                var dumpDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "AE_Dumps"
                );
                Directory.CreateDirectory(dumpDir);
                var fileName = Path.Combine(dumpDir, "targets_latest.txt");
                File.WriteAllText(fileName, sb.ToString());
            }
            catch (Exception ex)
            {
                Loader.Log($"AE dump write failed: {ex.Message}");
            }
        }
    }
}
