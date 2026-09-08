using HarmonyLib;
using LBoL.Presentation.UI.Panels;
using LongNightTimeLimit.UI;

namespace LongNightTimeLimit.Patches
{
    [HarmonyPatch(typeof(CardUi), "Awake")]
    class CardUiPatches
    {
        static void Postfix(CardUi __instance)
        {
            ReshuffleCounterBadge.AttachTo(__instance);
        }
    }
}
