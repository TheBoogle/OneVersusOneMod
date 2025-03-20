using Comfort.Common;
using EFT;
using Fika.Core.Coop.ObservedClasses;
using OneVersusOne.Services;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
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
