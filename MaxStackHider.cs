using BepInEx;
using HarmonyLib;

namespace MaxStackHider
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class MaxStackHider : BaseUnityPlugin
    {
        public const string GUID = "Turbero.MaxStackHider";
        public const string NAME = "Max Stack Hider";
        public const string VERSION = "1.0.0";

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            ConfigurationFile.LoadConfig(this);
            harmony.PatchAll();
        }

        void onDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
