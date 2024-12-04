using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MaxStackHider
{
    /*[HarmonyPatch(typeof(Player), "OnInventoryChanged")]
    public static class OnInventoryChangedPatch
    {
        public static void Postfix(Player __instance)
        {
            Logger.Log("**Player.OnInventoryChanged");
        }
    }*/

    public class InventoryPatch
    {

        [HarmonyPatch(typeof(InventoryGui), "Show")]
        public static class InventoryGuiShowPatch
        {
            public static void Postfix(InventoryGui __instance, Container container, int activeGroup = 1)
            {
                Logger.Log("**InventoryGui.InventoryGuiShowPatch");
                _ = updateInventoryStackNumbersAwait(0.1f, Player.m_localPlayer.GetInventory(), true);
                if (container != null)
                    _ = updateInventoryStackNumbersAwait(0.1f, container.GetInventory(), false);
            }
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveItemToThis), typeof(Inventory), typeof(ItemDrop.ItemData),
            typeof(int), typeof(int), typeof(int))]
        public static class MoveItemToThisBoolPatch
        {
            public static void Postfix(Inventory __instance, Inventory fromInventory, ItemDrop.ItemData item, int amount, int x, int y, ref bool __result)
            {   
                Logger.Log("**Inventory.MoveItemToThis Bool");
                if (__instance.GetName() == fromInventory.GetName())
                {
                    //Move between same container
                    _ = updateInventoryStackNumbersAwait(0.1f, __instance, __instance.GetName() == "Inventory");
                }
                else
                {
                    //Move between different containers
                    _ = updateInventoryStackNumbersAwait(0.1f, __instance, __instance.GetName() == "Inventory");
                    _ = updateInventoryStackNumbersAwait(0.1f, fromInventory, fromInventory.GetName() == "Inventory");
                }
                
            }
        }
        
        //private bool AddItem(ItemDrop.ItemData item, int amount, int x, int y)

        private static async Task updateInventoryStackNumbersAwait(float seconds, Inventory inventory, bool isPlayerInventory)
        {
            await Task.Delay((int)(Math.Max(0f, seconds) * 1000)); // to milisegundos
            updateInventoryStackNumbers(inventory, isPlayerInventory);
        }

        private static void updateInventoryStackNumbers(Inventory inventory, bool isPlayerInventory)
        {
            Transform inventoryRoot = isPlayerInventory
                ? InventoryGui.instance.transform.Find("root/Player/PlayerGrid/Root")
                : InventoryGui.instance.transform.Find("root/Container/ContainerGrid/Root");
            if (inventoryRoot.GetChild(0) == null) return; //Not initialized yet
            
            int totalColumns = (int)typeof(Inventory).GetField("m_width", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(inventory);
            int totalRows = (int)typeof(Inventory).GetField("m_height", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(inventory);

            for (int i = 0; i < totalRows; i++)
            {
                for (int j = 0; j < totalColumns; j++)
                {
                    if (inventoryRoot.GetChild(i * totalColumns + j).transform.Find("amountQuantity") == null)
                    {
                        createAmountQuantity(inventoryRoot.GetChild(i * totalColumns + j));
                    }
                    
                    ItemDrop.ItemData itemData = inventory.GetItemAt(j, i);
                    // No object in slot or not stackable item
                    if (itemData == null || itemData.m_shared.m_maxStackSize == 1) 
                    {
                        inventoryRoot.GetChild(i * totalColumns + j).transform.Find("amountQuantity").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                    }
                    else
                    {
                        inventoryRoot.GetChild(i * totalColumns + j).transform.Find("amountQuantity").gameObject.GetComponent<TextMeshProUGUI>().text = "" + itemData.m_stack;
                    }
                }
            }
        }

        private static void createAmountQuantity(Transform child)
        {
            RectTransform transformAmount = child.transform.Find("amount") as RectTransform;
            transformAmount.gameObject.SetActive(false);
            
            if (child.transform.Find("amountQuantity")?.gameObject == null)
            {
                GameObject newTextQuantityGo = new GameObject("amountQuantity");
                newTextQuantityGo.transform.SetParent(child.transform, false);

                RectTransform rectTransform = newTextQuantityGo.AddComponent<RectTransform>();
                rectTransform.sizeDelta = transformAmount.sizeDelta;
                rectTransform.anchoredPosition = new Vector2(0, -24);

                TextMeshProUGUI newText = newTextQuantityGo.AddComponent<TextMeshProUGUI>();
                newText.alignment = TextAlignmentOptions.Center;
                newText.fontSize = transformAmount.GetComponent<TextMeshProUGUI>().fontSize;
                newText.fontStyle = transformAmount.GetComponent<TextMeshProUGUI>().fontStyle;
                newText.font = transformAmount.GetComponent<TextMeshProUGUI>().font;
                newText.fontMaterial = transformAmount.GetComponent<TextMeshProUGUI>().fontMaterial;
            }
        }

        private static void updateChestInventoryStackNumbers(Inventory inventory)
        {
            
        }
    }
}