using System;

namespace LongNightTimeLimit
{
    /// <summary>
    /// Helper class to handle reshuffles.
    /// </summary>
    public static class ReshuffleTracker
    {
        /// <summary>Reshuffles counted so far this turn. Free ones are included.</summary>
        public static int ReshufflesThisTurn { get; private set; }

        public static void Reset()
        {
            ReshufflesThisTurn = 0;
        }

        public static void NotifyReshuffled()
        {
            ReshufflesThisTurn += 1;
        }

        /// <summary>
        /// True if the reshuffle we just counted should apply Time Limit.
        /// </summary>
        public static bool Penalize()
        {
            return IsPenalty(ReshufflesThisTurn);
        }

        /// <summary>
        /// How many more reshuffles the player can make before Time Limit is applied.
        /// </summary>
        public static int ReshufflesUntilPenalty()
        {
            int free = Math.Max(0, BepinexPlugin.ConfigFreeReshuffles);
            int every = Math.Max(1, BepinexPlugin.ConfigEveryXReshuffles);
            int done = ReshufflesThisTurn;

            // Penalties land on reshuffle number free+every, free+2*every, and so on.
            if (done < free)
            {
                return free + every - done;
            }

            int past = done - free;
            int nextPenalty = free + (past / every + 1) * every;
            return nextPenalty - done;
        }

        private static bool IsPenalty(int reshuffleNumber)
        {
            int free = Math.Max(0, BepinexPlugin.ConfigFreeReshuffles);
            int every = Math.Max(1, BepinexPlugin.ConfigEveryXReshuffles);

            return reshuffleNumber > free && (reshuffleNumber - free) % every == 0;
        }
    }
}
