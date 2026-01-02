using HarmonyLib;
using LBoL.Core;
using System;
using System.Text;

namespace LongNightTimeLimitMod_Windows.Source.Patches
{
    [HarmonyPatch(typeof(PuzzleFlagDisplayWord))]
    [HarmonyPatch(nameof(PuzzleFlagDisplayWord.Description))]
    [HarmonyPatch(MethodType.Getter)]
    class PuzzleFlagDisplayWordPatches
    {
        static void Postfix(ref string __result)
        {
            // Hacky way to override localization only for English for now, will fix later if possible
            if (__result.StartsWith("At the start of combat"))
            {
                __result = "Whenever you reshuffle the discard pile into the draw pile during your turn, gain Time Limit <sprite=\"ManaSprite\" name=\"1\"> (only after the draw step).";
            }
        }
    }
}
