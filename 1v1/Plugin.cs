using BepInEx;
using BepInEx.Bootstrap;
using Newtonsoft.Json;
using SPT.Common.Http;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace OneVersusOne.Patches
{
    [BepInPlugin("com.boogle.oneversusone", "OneVersusOne Mod", "1.0.0")]
    public class OneVersusOnePlugin : BaseUnityPlugin
    {
        public interface IFuckingConfig
        {
        }

        public class LocationConfig : IFuckingConfig
        {
            public string Name { get; set; }
            public Vector3 HostPosition { get; set; }
            public Vector3 ClientPosition { get; set; }
        }

        public class OneVersusOneConfig : IFuckingConfig
        {
            public LocationConfig[] Locations { get; set; }
        }

        public static OneVersusOneConfig ModConfig;

        private static T UpdateInfoFromServer<T>(string route) where T : class, IFuckingConfig
        {
            var json = RequestHandler.GetJson(route);

            return JsonConvert.DeserializeObject<T>(json);
        }

        private void Awake()
        {
            try
            {
                ModConfig = UpdateInfoFromServer<OneVersusOneConfig>("/OneVersusOne/GetConfig");
                new PreventDeath().Enable();
                DelayFikaLoad();
            }
            catch (Exception ex)
            {
                Logger.LogError($"A PATCH IN {GetType().Name} FAILED. SUBSEQUENT PATCHES HAVE NOT LOADED");
                Logger.LogError($"{GetType().Name}: {ex}");
                throw;
            }
            Logger.LogInfo($"Completed: {GetType().Name}");
        }

        private async void DelayFikaLoad()
        {
            await Task.Delay(5 * 1000);

            bool FikaLoaded = Chainloader.PluginInfos.ContainsKey("com.fika.core");

            if (FikaLoaded) // Fika patch
            {
                Logger.LogInfo("Fika loaded, applying Fika patch.");
                try
                {
                    new ObservedDeathPatch().Enable();
                    Logger.LogInfo("Fika patch applied.");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"A FIKA PATCH IN {GetType().Name} FAILED. SUBSEQUENT PATCHES HAVE NOT LOADED");
                    Logger.LogError($"{GetType().Name}: {ex}");
                    throw;
                }
            }
            else
            {
                Logger.LogInfo("Skipping Fika patch because Fika is not loaded.");
            }
        }
    }
}
