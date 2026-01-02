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


        void Awake()
        {
            log = Logger;
            // very important. Without this the entry point MonoBehaviour gets destroyed
            DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            log.LogInfo("Running Harmony patches for LongNightTimeLimitMod");
            harmony.PatchAll();
            log.LogInfo("Harmony patches for LongNightTimeLimitMod run");
        }

        void OnDestroy()
        {
            if (harmony != null)
            { 
                harmony.UnpatchSelf();
            }
        }
    }
}
