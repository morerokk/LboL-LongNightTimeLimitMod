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
            ReshuffleTracker.Reset();

            // Only run if Long Night is enabled
            if (__instance.GameRun.Puzzles.HasFlag(PuzzleFlag.NightMana))
            {
                // Reset counters
                __instance.Player.TurnStarting.AddHandler((UnitEventArgs args) => HandlePlayerTurnStarting(args, __instance), GameEventPriority.ConfigDefault);

                // Add Time Limit on reshuffle
                __instance.Reshuffled.AddHandler((GameEventArgs args) => HandleReshuffle(args, __instance), GameEventPriority.ConfigDefault);

                // Cancel any attempts to add Long Night to the hand
                __instance.CardsAddingToHand.AddHandler((CardsEventArgs args) => HandleCardsAddingToHand(args, __instance), GameEventPriority.Highest);
            }
        }

        private static void HandlePlayerTurnStarting(UnitEventArgs args, BattleController controller)
        {
            ReshuffleTracker.Reset();
        }

        private static void HandleReshuffle(GameEventArgs args, BattleController controller)
        {
            if (controller.BattleShouldEnd)
            {
                return;
            }

            // Check for draw step freebies
            if (!controller.Player.IsInTurn && BepinexPlugin.ConfigDrawStepIsFree)
            {
                return;
            }

            ReshuffleTracker.NotifyReshuffled();

            // Check for free reshuffles.
            if (!ReshuffleTracker.Penalize())
            {
                return;
            }

            controller.React(new Reactor(new ApplyStatusEffectAction<TimeIsLimited>(controller.Player, BepinexPlugin.ConfigTimeLimitStackGainCount)), controller.Player, ActionCause.JadeBox); // Is JadeBox the right actioncause for this? There is no Puzzle cause but this feels close enough
        }

        private static void HandleCardsAddingToHand(CardsEventArgs args, BattleController controller)
        {
            if (args.Cards.Any(card => card.Id.StartsWith("NightMana")))
            {
                args.CancelBy(controller.Player);
            }
        }
    }
}
