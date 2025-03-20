using BepInEx;
using BepInEx.Bootstrap;
using EFT;
using EFT.HealthSystem;
using EFT.Interactive;
using EFT.InventoryLogic;
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
    public class PreventDeath : ModulePatch
    {
        protected static double LastDeathMessage = 0;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(ActiveHealthController).GetMethod("Kill", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance)
        {
            try
            {

                var LastDamageInfoField = __instance.Player.GetType().GetField("LastDamageInfo", BindingFlags.NonPublic | BindingFlags.Instance);
                DamageInfoStruct LastDamageInfo = (DamageInfoStruct)LastDamageInfoField.GetValue(__instance.Player);

                if (__instance.Player.IsYourPlayer)
                {
                    OneVersusOneService.HealAndTeleport(__instance.Player);
                }

                __instance.method_34(LastDamageInfo.DamageType);

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError($"GenericPatch Prefix failed: {ex}");
                return true;
            }
        }

    }
}
