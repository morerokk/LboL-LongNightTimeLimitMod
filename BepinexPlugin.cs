using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace LongNightTimeLimit
{
    [BepInPlugin(LongNightTimeLimit.PInfo.GUID, LongNightTimeLimit.PInfo.Name, LongNightTimeLimit.PInfo.version)]
    [BepInProcess("LBoL.exe")]
    public class BepinexPlugin : BaseUnityPlugin
    {
        //The Unique mod ID of the mod.
        //If defined, this is also the ID used by the Act 1 boss.
        //WARNING: It is mandatory to rename it to avoid issues.
        public static string modUniqueID = "LongNightTimeLimitMod";

        private static readonly Harmony harmony = LongNightTimeLimit.PInfo.harmony;

        internal static BepInEx.Logging.ManualLogSource log;

        /// <summary>
        /// Amount of reshuffles that are ignored before Time Limit starts being added.
        /// </summary>
        private static ConfigEntry<int> FreeReshuffles;
        /// <summary>
        /// If true, reshuffling during the draw step is always considered free and won't be counted.
        /// </summary>
        private static ConfigEntry<bool> DrawStepIsFree;
        /// <summary>
        /// 1 in X reshuffles will add Time Limit, where X is this value.
        /// </summary>
        private static ConfigEntry<int> EveryXReshuffles;
        /// <summary>
        /// The amount of Time Limit to be added.
        /// </summary>
        private static ConfigEntry<int> TimeLimitStackGainCount;
        /// <summary>
        /// If true, cards that trigger reshuffles forcibly when played (such as Gifts Given) are considered free and won't be counted.
        /// (Currently unused)
        /// </summary>
        private static ConfigEntry<bool> GiftsGivenIsFree;

        public static int ConfigFreeReshuffles => FreeReshuffles.Value;
        public static bool ConfigDrawStepIsFree => DrawStepIsFree.Value;
        public static int ConfigEveryXReshuffles => EveryXReshuffles.Value;
        public static int ConfigTimeLimitStackGainCount => TimeLimitStackGainCount.Value;
        public static bool ConfigGiftsGivenIsFree => GiftsGivenIsFree.Value;


        void Awake()
        {
            log = Logger;
            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            log.LogDebug($"Setting up config for LongNightTimeLimitMod v{LongNightTimeLimit.PInfo.version}");
            SetupConfig();
            log.LogDebug("Done setting up config for LongNightTimeLimitMod");

            log.LogDebug("Running Harmony patches for LongNightTimeLimitMod");
            harmony.PatchAll();
            log.LogDebug("Done running Harmony patches for LongNightTimeLimitMod");
        }

        void OnDestroy()
        {
            if (harmony != null)
            { 
                harmony.UnpatchSelf();
            }
        }

        private void SetupConfig()
        {
            FreeReshuffles = Config.Bind("Tweaks", nameof(FreeReshuffles), 1, "Amount of reshuffles that are ignored before Time Limit starts being added.");
            DrawStepIsFree = Config.Bind("Tweaks", nameof(DrawStepIsFree), false, "If true, reshuffling during the draw step is always considered free and won't be counted.");
            EveryXReshuffles = Config.Bind("Tweaks", nameof(EveryXReshuffles), 1, "1 in X reshuffles will add Time Limit, where X is this value. Only starts counting after the free reshuffles are exhausted.");
            TimeLimitStackGainCount = Config.Bind("Tweaks", nameof(TimeLimitStackGainCount), 1, "The amount of Time Limit to be added.");
            GiftsGivenIsFree = Config.Bind("Tweaks", nameof(GiftsGivenIsFree), true, "If true, cards that trigger reshuffles forcibly when played (such as Gifts Given) are considered free and won't be counted. (Currently unused because work in progress, it's unclear if this card even counts in the first place)");
        }
    }
}
