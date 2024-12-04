using HarmonyLib;

namespace MaxStackHider
{
    public class LoggerEmptyPatch
    {
        [HarmonyPatch(typeof(Player), "OnInventoryChanged")]
        public static class OnInventoryChangedPatch
        {
            public static void Postfix(Player __instance)
            {
                Logger.Log("**Player.OnInventoryChanged");
            }
        }
        
        [HarmonyPatch(typeof(InventoryGui), "OnDropOutside")]
        public static class OnDropOutsidePatch
        {
            public static void Postfix(InventoryGui __instance)
            {
                Logger.Log("**InventoryGui.OnDropOutside");
            }
        }
        
        [HarmonyPatch(typeof(Humanoid), "DropItem")]
        public static class DropItemPatch
        {
            public static void Postfix(Humanoid __instance)
            {
                Logger.Log("**Humanoid.DropItemPatch");
            }
        }
    }
}