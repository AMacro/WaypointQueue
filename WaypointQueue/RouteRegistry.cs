using System;
using System.Collections.Generic;
using System.Linq;

namespace WaypointQueue
{
    public static class RouteRegistry
    {
        public static List<RouteDefinition> Routes { get; private set; } = new List<RouteDefinition>();

        public static event Action OnChanged;

        private static void RaiseChanged()
        {
            OnChanged?.Invoke();
        }
        public static void ReloadFromDisk()
        {
            Routes = RouteSaveManager.LoadAll();
            RaiseChanged();
        }

        public static void ReplaceAll(List<RouteDefinition> routes)
        {
            Routes = routes ?? new List<RouteDefinition>();
            RaiseChanged();
        }

        public static RouteDefinition GetById(string id) => Routes.FirstOrDefault(r => r.Id == id);

        public static void Add(RouteDefinition def, bool save = true)
        {
            Routes.Add(def);
            if (save) RouteSaveManager.Save(def);
            RaiseChanged();
        }

        public static void Remove(string id, bool deleteFile = true)
        {
            var r = GetById(id);
            if (r == null) return;
            if (deleteFile) RouteSaveManager.Delete(r);
            Routes.RemoveAll(x => x.Id == id);
            RaiseChanged();
        }

        public static void Save(RouteDefinition r)
        {
            RouteSaveManager.Save(r);
        }
        public static void SaveAs(RouteDefinition r, string newName)
        {
            RouteSaveManager.SaveAs(r, newName);
            RaiseChanged();
        }
        public static void Rename(RouteDefinition r, string newName, bool moveFile = true)
        {
            RouteSaveManager.Rename(r, newName, moveFile);
            RaiseChanged();
        }
    }
}
