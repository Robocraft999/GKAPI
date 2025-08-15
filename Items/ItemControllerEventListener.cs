using Gatekeeper.Char_Scripts.General;
using Gatekeeper.Char_Scripts.General.CharData;
using Gatekeeper.Enemy.Base;
using Gatekeeper.General.Events.Characters;
using Gatekeeper.General.Events.Enemies;
using Gatekeeper.General.Events.Interactable;
using Gatekeeper.Utility;
using HarmonyLib;
using UnityEngine;

namespace GKAPI.Items;

[HarmonyPatch]
public class ItemControllerEventListener
{
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerHandleSkillUsed))]
    [HarmonyPostfix]
    private static void OnSkillUsed(EventClientCharacterSkillUsed eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerHandleSkillUsed(eventData);
        }
    }

    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerHandleOnEnemyDied))]
    private static void OnEnemyDied(EventClientEnemyDied eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerHandleOnEnemyDied(eventData);
        }
    }
    
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.ClientHandleInteractableObjectUsed))]
    private static void OnInteractableUsed(EventClientInteractableObjectUsed eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.ClientHandleInteractableUsed(eventData);
        }
    }
    
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerHandleDamageTaken))]
    private static void OnDamageTaken(DamageType damageType, float damageTaken)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerHandleDamageTaken(damageType, damageTaken);
        }
    }
    
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerHandleCharacterHitSomething))]
    private static void OnCharacterHitSomething(EventOwnerCharacterHitSomething eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerHandleCharacterHitSomething(eventData);
        }
    }
    
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerFirstSkillHit))]
    private static void OnFirstSkillHit(IEnemy enemy, EventOwnerCharacterHitSomething eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerHandleFirstSkillHit(enemy, eventData);
        }
    }
    
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerSkillHit))]
    private static void OnSkillHit(IEnemy enemy, EventOwnerCharacterHitSomething eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerSkillHit(enemy, eventData);
        }
    }
    
    [HarmonyPatch(typeof(CharItemCaster), nameof(CharItemCaster.OwnerItemHit))]
    private static void OnItemHit(IEnemy enemy, EventOwnerCharacterHitSomething eventData)
    {
        foreach (var controller in ItemAPI.Instance.GetItemControllers())
        {
            controller.OwnerItemHit(enemy, eventData);
        }
    }
    
    [HarmonyPatch(typeof(CharItemManager), nameof(CharItemManager.ClientInit))]
    [HarmonyPostfix]
    private static void OnCharacterLoad(CharItemManager __instance)
    {
        Plugin.Log.LogInfo($"Appending ItemControllers");
        var itemApi = ItemAPI.Instance;
        var container = __instance.transform.GetChild(0);

        var charManager = __instance.gameObject.GetComponent<CharData>();

        foreach (var (itemID, controllerType) in itemApi.itemControllerTypes)
        {
            var go = new GameObject($"Controller - {itemApi.GetItemById(itemID).name}");
            var component = (CustomItemController)go.AddComponent(controllerType);
            component.ItemID = itemID;
            component.charData = charManager;
            //component.manager = charManager;
            
            go.transform.SetParent(container.transform);
            itemApi.itemControllers[itemID] = component;
        }
    }
}