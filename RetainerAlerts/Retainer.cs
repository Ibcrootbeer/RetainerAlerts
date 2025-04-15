using System;
using System.Collections.Generic;
using System.Linq;

using FFXIVClientStructs.FFXIV.Client.Game;

namespace RetainerAlerts
{
    internal unsafe class Retainer
    {
        private static readonly RetainerManager* RetainerManager = FFXIVClientStructs.FFXIV.Client.Game.RetainerManager.Instance();
        private static bool RetainersExist = false;
        // TODO I hate this
        public static List<RetainerManager.Retainer> Retainers = RetainerManager->Retainers.ToArray().ToList().Where(x => x.Available).ToList();

        public static bool AnyVenturesComplete()
        {
            foreach (var retainer in Retainers)
            {
                if (retainer.VentureComplete == 0 || DateTimeOffset.FromUnixTimeSeconds(retainer.VentureComplete) < DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsRetainerDone(RetainerManager.Retainer retainer)
        {
            return retainer.VentureComplete == 0 || DateTimeOffset.FromUnixTimeSeconds(retainer.VentureComplete) < DateTime.Now;
        }

        public static bool AreAllVenturesComplete()
        {
            foreach (var retainer in Retainers)
            {
                if (retainer.VentureComplete > 0 && DateTimeOffset.FromUnixTimeSeconds(retainer.VentureComplete) > DateTime.Now)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool UpdateRetainers()
        {
            var newRetaienrs = RetainerManager->Retainers.ToArray().ToList().Where(x => x.Available);
            if (newRetaienrs.Any())
            {
                Retainers = newRetaienrs.ToList();
                return true;
            }
            return false;
        }
    }
}
