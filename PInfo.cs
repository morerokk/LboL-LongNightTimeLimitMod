using HarmonyLib;

namespace LongNightTimeLimit
{
    public static class PInfo
    {
        public const string GUID = "rokk.lbol.gameplay.LongNightTimeLimit";
        public const string Name = "LongNightTimeLimit";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
