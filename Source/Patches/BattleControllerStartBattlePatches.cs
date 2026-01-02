using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.EntityLib.StatusEffects.ExtraTurn;
using System.Linq;

namespace LongNightTimeLimit.Patches
{
    [HarmonyPatch(typeof(BattleController), nameof(BattleController.StartBattle))]
    class BattleControllerStartBattlePatches
    {
        static void Postfix(BattleController __instance)
        {
            // Only run if Long Night is enabled
            if (__instance.GameRun.Puzzles.HasFlag(PuzzleFlag.NightMana))
            {
                // Add Time Limit on reshuffle
                __instance.Reshuffled.AddHandler((GameEventArgs args) =>
                {
                    // Only add Time Limit if the player is currently playing their turn (i.e. not during the draw step or turn end)
                    if (!__instance.BattleShouldEnd && __instance.Player.IsInTurn)
                    {
                        __instance.React(new Reactor(new ApplyStatusEffectAction<TimeIsLimited>(__instance.Player, 1)), __instance.Player, ActionCause.JadeBox);
                    }
                    ;
                }, GameEventPriority.ConfigDefault);

                // Cancel any attempts to add Long Night to the hand
                __instance.CardsAddingToHand.AddHandler((CardsEventArgs args) => {
                    if (args.Cards.Any(card => card.Id.StartsWith("NightMana")))
                    {
                        args.CancelBy(__instance.Player);
                    }
                }, GameEventPriority.Highest);
            }
        }
    }
}
