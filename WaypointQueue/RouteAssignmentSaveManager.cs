using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using WaypointQueue.UUM;

namespace WaypointQueue
{
    public static class RouteAssignmentSaveManager
    {
        private const float SaveDebounceSeconds = 0.35f;
        private static float _sinceDirty = 0f;
        private static bool _dirty = false;

        private static string Dir => Path.Combine(Application.persistentDataPath, "Routes");
        private static string FilePath => Path.Combine(Dir, "route_assignments.json");

        [Serializable]
        private class AssignmentFile
        {
            public int version = 1;
            public List<RouteAssignment> items = new List<RouteAssignment>();
        }

        static RouteAssignmentSaveManager()
        {
            RouteAssignmentRegistry.OnChanged += () =>
            {
                _dirty = true;
                _sinceDirty = 0f;
            };
        }

        public static void ReloadFromDisk()
        {
            try
            {
                if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
                if (!File.Exists(FilePath))
                {
                    RouteAssignmentRegistry.ReplaceAll(null);
                    return;
                }

                var json = File.ReadAllText(FilePath);
                var data = JsonConvert.DeserializeObject<AssignmentFile>(json);
                RouteAssignmentRegistry.ReplaceAll(data?.items);
                Loader.Log($"[RouteAssign] Loaded {data?.items?.Count ?? 0} assignments.");
            }
            catch (Exception e)
            {
                Loader.Log($"[RouteAssign] Load failed: {e}");
                RouteAssignmentRegistry.ReplaceAll(null);
            }
        }

        public static void Update(float deltaTime)
        {
            if (!_dirty) return;

            _sinceDirty += deltaTime;
            if (_sinceDirty < SaveDebounceSeconds) return;

            try
            {
                if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
                var data = new AssignmentFile
                {
                    version = 1,
                    items = RouteAssignmentRegistry.All()
                };
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(FilePath, json);
                Loader.Log($"[RouteAssign] Saved {data.items.Count} assignments → {FilePath}");
            }
            catch (Exception e)
            {
                Loader.Log($"[RouteAssign] Save failed: {e}");
            }
            finally
            {
                _dirty = false;
                _sinceDirty = 0f;
            }
        }
    }
}
