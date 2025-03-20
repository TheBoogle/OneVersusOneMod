using EFT.HealthSystem;
using OneVersusOne.Services;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

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

                    MineDirectional[] AllMines = UnityEngine.Object.FindObjectsOfType<MineDirectional>();

                    foreach (var item in AllMines)
                    {
                        var bool_1field = item.GetType().GetField("bool_1", BindingFlags.NonPublic | BindingFlags.Instance);

                        if (bool_1field != null && (bool)bool_1field.GetValue(item) == true)
                        {
                            bool_1field.SetValue(item, false);

                            item.SetArmed(true);
                            item.gameObject.SetActive(true);
                        }
                    }
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
