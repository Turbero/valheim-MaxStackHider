using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MaxStackHider
{
    [HarmonyPatch(typeof(UITooltip), "UpdateTextElements")]
    public static class UITooltipPatch
    {
        public static void Postfix(UITooltip __instance)
        {
            GameObject m_tooltip = (GameObject)typeof(UITooltip).GetField("m_tooltip", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(__instance);
            if (m_tooltip != null)
            {
                Transform transform = Utils.FindChild(m_tooltip.transform, "Text");
                if (transform != null)
                {
                    //1 - Update quantity - Hide existing
                    TextMeshProUGUI text = __instance.transform.Find("amount")?.GetComponent<TextMeshProUGUI>();
                    if (text == null) return;

                    if (text.text == "1/3") return; // Default empty amount when not applicable.
                    
                    string[] quantities = text.text.Split('/');
                    
                    if (__instance.transform.Find("amount") == null) return;
                    RectTransform transformAmount = __instance.transform.Find("amount") as RectTransform;
                    transformAmount.gameObject.SetActive(false);
                    
                    //2 - Update quantity - Add new element with only current quantity and no stack info
                    TextMeshProUGUI newText;
                    if (__instance.transform.Find("amountQuantity")?.gameObject == null)
                    {
                        GameObject newTextQuantityGo = new GameObject("amountQuantity");
                        newTextQuantityGo.transform.SetParent(__instance.transform, false);

                        RectTransform rectTransform = newTextQuantityGo.AddComponent<RectTransform>();
                        rectTransform.sizeDelta = transformAmount.sizeDelta;
                        rectTransform.anchoredPosition = new Vector2(0, -24);

                        newText = newTextQuantityGo.AddComponent<TextMeshProUGUI>();
                        newText.alignment = TextAlignmentOptions.Center;
                        newText.fontSize = transformAmount.GetComponent<TextMeshProUGUI>().fontSize;
                        newText.fontStyle = transformAmount.GetComponent<TextMeshProUGUI>().fontStyle;
                        newText.font = transformAmount.GetComponent<TextMeshProUGUI>().font;
                        newText.fontMaterial = transformAmount.GetComponent<TextMeshProUGUI>().fontMaterial;
                    }
                    else
                    {
                        GameObject newTextQuantityGo = __instance.transform.Find("amountQuantity")?.gameObject;
                        newText = newTextQuantityGo.GetComponent<TextMeshProUGUI>();
                    }

                    newText.text = quantities[0];
                    
                    //3 - Add max weight to tooltip
                    string weight = $"\n{ConfigurationFile.maxQuantityText.Value}: <color=\"orange\">"+quantities[1]+"</color>";
                    __instance.m_text = __instance.m_text.Replace("_description", "_description " + weight);
                    transform.GetComponent<TMP_Text>().text = Localization.instance.Localize(__instance.m_text);
                }
            }
        }
    }
}