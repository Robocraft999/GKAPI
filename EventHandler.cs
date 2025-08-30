using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Gatekeeper.Achievements.Data;
using Gatekeeper.CameraScripts.HUD.Items;
using Sirenix.Utilities;
using Gatekeeper.CameraScripts.HUD.ObeliskShopPopup;
using Gatekeeper.CameraScripts.HUD.Triads;
using Gatekeeper.Char_Scripts;
using Gatekeeper.Char_Scripts.General;
using Gatekeeper.General;
using Gatekeeper.General.SaveLoad;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Items;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using Gatekeeper.MainMenuScripts.MainMenu.MainMenuPanel;
using HarmonyLib;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GKAPI;

[HarmonyPatch]
public class EventHandler
{
    public static LoadingState State { get; private set; } = LoadingState.PreInit;
    public static Action Init;
    public static Action LateInit;
    public static Action StartGame;
    public static Action<string> LoadScene;

    public static void OnLoad()
    {
        if (State == LoadingState.PreInit)
        {
            State = LoadingState.Init;
            Init?.Invoke();
            SceneManager.sceneLoaded += (UnityAction<Scene, LoadSceneMode>) OnSceneLoad;
        }
        else
            Plugin.Log.LogError("Invalid loading state");
    }

    [HarmonyPatch(typeof(MainMenuController), nameof(MainMenuController.Awake))]
    [HarmonyPostfix]
    private static void OnMenuLoad()
    {
        if (State != LoadingState.Init)
            return;
        
        State = LoadingState.LateInit;
        LateInit?.Invoke();
    }

    private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        LoadScene?.Invoke(scene.name);
    }

    /*[HarmonyPatch(typeof(GameplayManager), nameof(GameplayManager.ClientInit))]
    [HarmonyPostfix]
    private static void OnGameStart()
    {
        StartGame?.Invoke();
    }*/

    //[HarmonyPatch(typeof(TriadPanel), nameof(TriadPanel.Setup))]
    //[HarmonyPrefix]
    /*private static void OnSetupTriadPanel(TriadPanel __instance, ItemDatabaseInfo itemInfo, ItemLocalizer itemLocalizer)
    {
        Plugin.Log.LogInfo($"Triad Panel Setup Prefix Pre: '{__instance == null}'");
        var this_inited = __instance._inited;
        var this_itemImagesControllers = __instance.itemImagesControllers;
        var this_itemDescriptionTooltips = __instance.itemDescriptionTooltips;
        ItemLocalizer this_itemLocalizer = __instance._itemLocalizer;
        Plugin.Log.LogInfo("Triad Panel Setup Prefix Start");
        if (this_inited == false)
        {
            var itemImagesControllers = this_itemImagesControllers;
            this_inited = true;
            var initializer = TriadPanel.__c.__9__10_0;
            if (initializer == null)
            {
                var unknown2 = TriadPanel.__c.__9;
                var action = new Action<TriadImageController>(unknown2._Init_b__10_0);
                initializer = action;
            }

            foreach (var itemImageController in itemImagesControllers)
            {
                initializer.Invoke(itemImageController);
            }
            //LinqExtensions.ForEach(itemImagesControllers.WrapToIl2Cpp().Cast<Il2CppSystem.Collections.Generic.IEnumerable<TriadImageController>>(), initializer);
            this_itemLocalizer = itemLocalizer;
        }
        Plugin.Log.LogInfo($"Triad Panel Setup Prefix itemInfo null? '{itemInfo == null}'");

        var this_itemInfo = itemInfo;
        var items = DatabaseInfoProvider.Items;
        var this_itemsInfoProvider = items;
        var _itemsInfoProvider = this_itemsInfoProvider;
        var _itemInfo = this_itemInfo;
        var ItemRelatedDatas = _itemsInfoProvider.ItemRelatedDatas;
        var item = ItemRelatedDatas.get_Item(_itemInfo.ItemId);
        var this_triadInfo = item.TriadInfo;
        var _triadInfo = this_triadInfo;
        Plugin.Log.LogInfo($"Triad Panel Setup Prefix triadInfo null? '{_triadInfo == null}'");
        if (_triadInfo == null)
        {
            return;
        }

        var itemImagesControllers2 = this_itemImagesControllers;
        var local21 = 0;
        var allItemsUnlocked = true;
        while (local21 < itemImagesControllers2.Length)
        {
            Plugin.Log.LogInfo($"Triad Panel Setup Prefix length {local21}, {this_itemImagesControllers.Length} {this_itemDescriptionTooltips.Length}");
            var _triadInfo2 = this_triadInfo;
            var triadItems = _triadInfo2.triadItems;
            var item2 = triadItems.get_Item(local21);
            var itemDescriptionTooltips = this_itemDescriptionTooltips;
            if (itemDescriptionTooltips.Length > local21)
            {
                var shrineOfTriadsItemDescriptionTooltip = itemDescriptionTooltips[local21];
                var _itemLocalizer = this_itemLocalizer;
                shrineOfTriadsItemDescriptionTooltip.FillTooltip(item2, _itemLocalizer);
            }
            var _itemsInfoProvider2 = this_itemsInfoProvider;
            var ItemRelatedDatas2 = _itemsInfoProvider2.ItemRelatedDatas;
            var item3 = ItemRelatedDatas2.get_Item(_itemInfo.ItemId);
            
            var itemImagesControllers3 = this_itemImagesControllers;
            var triadImageController = itemImagesControllers3[local21];
            triadImageController.SetGrayScale(0);
            
            var itemImagesControllers4 = this_itemImagesControllers;
            var triadImageController2 = itemImagesControllers4[local21];
            var local41 = 0;
            triadImageController2.SetGradBlend(local41);
            
            var achievements = DatabaseInfoProvider.Achievements;
            var isUnlocked = achievements.IsUnlocked(AchievementID.Triad);

            if (isUnlocked)
            {
                var itemImagesControllers5 = this_itemImagesControllers;
                var triadImageController3 = itemImagesControllers5[local21];
                var triadImage = triadImageController3.triadImage;
                var itemIcon = item2.DatabaseIcon;
                triadImage.sprite = itemIcon;
                var sstring = item2.Id;
                var unlockedStatus = SaveLoadManager.GetUnlockedStatus(sstring);
                if (unlockedStatus)
                {
                    var ItemID = item2.ItemId;
                    var local53 = PlayableCharactersHolder.MyCharacter.ItemManager;
                    var isItemInInventory = local53.IsItemInInventory(ItemID);
                    if (isItemInInventory)
                    {
                        var itemImagesControllers6 = this_itemImagesControllers;
                        var triadImageController4 = itemImagesControllers6[local21];
                        triadImageController4.SetGrayScale(1);
                        var itemDescriptionTooltips2 = this_itemDescriptionTooltips;
                        if (itemDescriptionTooltips2.Length > local21)
                        {
                            var shrineOfTriadsItemDescriptionTooltip2 = itemDescriptionTooltips2[local21];
                            shrineOfTriadsItemDescriptionTooltip2.SetItemAvailabilityState(ItemAvailabilityState.InInventory);
                        }
                    }
                    else
                    {
                        var itemDescriptionTooltips3 = this_itemDescriptionTooltips;
                        if (itemDescriptionTooltips3.Length > local21)
                        {
                            var shrineOfTriadsItemDescriptionTooltip3 = itemDescriptionTooltips3[local21];
                            shrineOfTriadsItemDescriptionTooltip3.SetItemAvailabilityState(ItemAvailabilityState.NonInInventory);
                        }
                    }
                    
                    var itemImagesControllers7 = this_itemImagesControllers;
                    var triadImageController5 = itemImagesControllers7[local21];
                    
                    triadImageController5.SetRarityImageState(true);
                    
                    var items2 = DatabaseInfoProvider.Items;
                    var itemType = item2.itemType;
                    var itemTypeInfo = items2.GetItemTypeInfo(itemType);
                    
                    var itemImagesControllers8 = this_itemImagesControllers;
                    var triadImageController6 = itemImagesControllers8[local21];
                    var TypeColor = itemTypeInfo.TypeColor;
                    
                    triadImageController6.SetRarityColor(TypeColor);
                }
                else
                {
                    var itemImagesControllers9 = this_itemImagesControllers;
                    var triadImageController7 = itemImagesControllers9[local21];
                    triadImageController7.SetGradBlend(1);
                    
                    var itemImagesControllers10 = this_itemImagesControllers;
                    var triadImageController8 = itemImagesControllers10[local21];
                    triadImageController8.SetRarityImageState(false);
                    
                    var itemDescriptionTooltips4 = this_itemDescriptionTooltips;
                    if (itemDescriptionTooltips4.Length > local21)
                    {
                        var shrineOfTriadsItemDescriptionTooltip4 = itemDescriptionTooltips4[local21];
                        shrineOfTriadsItemDescriptionTooltip4.SetItemAvailabilityState(ItemAvailabilityState.LockedInEmporium);
                    }
                }
            }
            else
            {
                var itemDescriptionTooltips4 = this_itemDescriptionTooltips;
                var shrineOfTriadsItemDescriptionTooltip4 = itemDescriptionTooltips4[local21];
                var this_info = __instance.info;
                var info = this_info;
                var LockedItemSprite = info.LockedItemSprite;
                var triadImage = triadImageController2.triadImage;
                triadImage.sprite = LockedItemSprite;
                
                var itemImagesControllers11 = this_itemImagesControllers;
                var triadImageController9 = itemImagesControllers11[local21];
                triadImageController9.SetRarityImageState(false);
                var itemDescriptionTooltips5 = this_itemDescriptionTooltips;
                if (itemDescriptionTooltips5.Length > local21)
                {
                    var shrineOfTriadsItemDescriptionTooltip5 = itemDescriptionTooltips5[local21];
                    shrineOfTriadsItemDescriptionTooltip5.SetItemAvailabilityState(ItemAvailabilityState.AchievementLocked);
                }
                allItemsUnlocked = false;
            }
            local21 += 1;
        }
        __instance.FillTooltip(allItemsUnlocked);
        Plugin.Log.LogInfo("Triad Panel Setup Prefix End");
        //Plugin.Log.LogInfo($"Triad Panel Setup '{__instance._triadInfo}'");
        //Plugin.Log.LogInfo($"   '{DatabaseInfoProvider.Items.ItemRelatedDatas.get_Item(itemInfo.ItemID).TriadInfo.id}'");
        //Plugin.Log.LogInfo($"Triad Panel Setup {__instance} {itemInfo.id} {itemLocalizer}");
    }*/
}

public enum LoadingState
{
    PreInit,
    Init,
    LateInit,
}