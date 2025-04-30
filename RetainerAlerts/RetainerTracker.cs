using System;
using System.Collections.Generic;
using System.Linq;

using FFXIVClientStructs.FFXIV.Client.Game;

using static FFXIVClientStructs.FFXIV.Client.Game.RetainerManager;

namespace RetainerAlerts
{
    internal unsafe class RetainerTracker
    {
        public static bool IsRetainerDone(Retainer retainer)
        {
            return retainer.VentureComplete == 0 || DateTimeOffset.FromUnixTimeSeconds(retainer.VentureComplete) < DateTime.Now;
        }

        public static bool AnyVenturesComplete()
        {
            var retainers = GetRetainers();
            if(retainers == null)
            {
                return false;
            }

            foreach (var retainer in retainers)
            {
                if (IsRetainerDone(retainer))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AreAllVenturesComplete()
        {
            var retainers = GetRetainers();
            if (retainers == null)
            {
                return false;
            }

            foreach (var retainer in retainers)
            {
                if (!IsRetainerDone(retainer))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AreAnyRetainersLoaded()
        {
            var retainers = GetRetainers();
            if (retainers is null)
            {
                return false;
            }
            return retainers.Count > 0;
        }

        private static List<Retainer>? GetRetainers()
        {
            var manager = RetainerManager.Instance();

            if (manager is null)
            {
                // TODO Consider logging this somewhere to show something has gone wrong?
                return null;
            }
            return manager->Retainers.ToArray().ToList().Where(x => x.Available).ToList();
        }
    }
}
