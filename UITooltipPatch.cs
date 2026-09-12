using HarmonyLib;

namespace MaxStackHider
{
    [HarmonyPatch(typeof(InventoryGrid), "CreateItemTooltip")]
    [HarmonyPriority(Priority.Last)]
    public static class UITooltipPatch
    {
        public static bool Prefix(InventoryGrid __instance, ItemDrop.ItemData item, UITooltip tooltip)
        {
            if (item.m_shared.m_maxStackSize == 1) return true;
            
            //Add max weight to tooltip
            string tooltipText = item.GetTooltip();
            string weight = $"\n{ConfigurationFile.maxQuantityText.Value}: <color=\"orange\">"+item.m_shared.m_maxStackSize+"</color>";
            tooltipText = tooltipText.Replace("_description", "_description " + weight);
            
            tooltip.Set(item.m_shared.m_name, tooltipText, __instance.m_tooltipAnchor);

            return false;
        }
    }
}