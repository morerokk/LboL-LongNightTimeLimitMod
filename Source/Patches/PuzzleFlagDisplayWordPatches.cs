using HarmonyLib;
using LBoL.Core;
using System;
using System.Text;

namespace LongNightTimeLimit.Patches
{
    [HarmonyPatch(typeof(PuzzleFlagDisplayWord))]
    [HarmonyPatch(nameof(PuzzleFlagDisplayWord.Description))]
    [HarmonyPatch(MethodType.Getter)]
    class PuzzleFlagDisplayWordPatches
    {
        static void Postfix(ref string __result)
        {
            // Hacky way to override localization only for English for now, will fix later if possible
            if (__result.StartsWith("At the start of combat, add a"))
            {
                // Lol
                StringBuilder tooltip = new StringBuilder("Whenever you reshuffle the discard pile into the draw pile");
                if (BepinexPlugin.ConfigFreeReshuffles > 0)
                {
                    tooltip.Append(" more than ");
                    tooltip.Append(BepinexPlugin.ConfigFreeReshuffles);
                    tooltip.Append(BepinexPlugin.ConfigFreeReshuffles != 1 ? " times" : " time");
                }
                tooltip.Append($" during your turn, gain Time Limit <sprite=\"ManaSprite\" name=\"{BepinexPlugin.ConfigTimeLimitStackGainCount}\">");
                if (BepinexPlugin.ConfigEveryXReshuffles > 1)
                {
                    tooltip.Append($" every {BepinexPlugin.ConfigEveryXReshuffles} reshuffles.");
                }
                else
                {
                    tooltip.Append(".");
                }

                if (BepinexPlugin.ConfigDrawStepIsFree)
                {
                    tooltip.Append(" Reshuffling during the draw step does not count towards this penalty.");
                }

                __result = tooltip.ToString();
            }
        }
    }
}
