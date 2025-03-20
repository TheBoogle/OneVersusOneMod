using BepInEx;
using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using EFT.Interactive;
using EFT.InventoryLogic;
using Fika.Core.Coop.ObservedClasses;
using Fika.Core.Coop.HostClasses;
using Fika.Core.Coop.Players;
using OneVersusOne.Services;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace OneVersusOne.Patches
{
    public class ObservedDeathPatch : ModulePatch
    {
        protected static double LastDeathMessage = 0;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(ObservedHealthController).GetMethod("method_21", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool Prefix(ObservedHealthController __instance, NetworkHealthSyncPacketStruct.GStruct376 data)
        {
            try
            {
                GameWorld GameWorld = Singleton<GameWorld>.Instance;

                OneVersusOneService.HealAndTeleport(GameWorld.MainPlayer);
                OneVersusOneService.RepairArmor(__instance.Player);

                string KillMessage = $"{__instance.Player.Profile.Nickname} died";

                if (Time.time - LastDeathMessage > 1)
                {
                    LastDeathMessage = Time.time;
                    NotificationManagerClass.DisplayMessageNotification(KillMessage, EFT.Communications.ENotificationDurationType.Long, EFT.Communications.ENotificationIconType.Alert, Color.red);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError($"GenericPatch Prefix failed: {ex}");
                return true;
            }
        }
    }
}
