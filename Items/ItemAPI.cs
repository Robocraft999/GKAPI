using System;
using System.Collections.Generic;
using System.Linq;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Items;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using GKAPI.Lang;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace GKAPI.Items;

public class ItemAPI
{
    private List<GkItem> _items = [];
    internal Dictionary<ItemID, Il2CppSystem.Type> itemControllerTypes = new();
    internal readonly Dictionary<ItemID, CustomItemController> itemControllers = new();

    private static List<int> _existingIds = System.Enum.GetValues<ItemID>().Cast<int>().ToList();
    private static int _nextItemId = 0;
    private static int NextItemId
    {
        get 
        {
            while (_existingIds.Contains(_nextItemId)) _nextItemId++;
            return _nextItemId++;
        }
    }

    public static ItemAPI Instance { get; } = new();

    private ItemAPI()
    {
        EventHandler.Init += RegisterItems;
    }

    //TODO add description and stats
    public GkItem AddItem(string name)
    {
        return AddItem(new GkItem.Builder(name));
    }

    public GkItem AddItem(string name, Func<GkItem.Builder, GkItem.Builder> build)
    {
        var builder = build(new GkItem.Builder(name));
        return AddItem(builder);
    }

    //TODO add description and (stats)
    public GkItem AddTriad(string name, List<ItemID> itemIds)
    {
        return AddItem(new GkItem.Builder(name));
    }

    public GkItem AddTriad(string name, List<ItemID> itemIds, Func<GkItem.Builder, GkItem.Builder> build)
    {
        var builder = build(new GkItem.Builder(name).AsTriad(itemIds));
        return AddItem(builder);
    }

    public GkItem AddItem(GkItem.Builder builder)
    {
        var item = builder.Build(NextItemId);
        _items.Add(item);
        return item;
    }

    public void AddItemController<T>(ItemID id)
    where T: CustomItemController
    {
        try
        {
            ClassInjector.RegisterTypeInIl2Cpp<T>();
            var controllerType = Il2CppType.Of<T>();
            itemControllerTypes.Add(id, controllerType);
        }
        catch
        {
            Plugin.Log.LogError("FAILED to Register Il2Cpp Type!");
        }
    }
    
    public ItemDatabaseInfo GetItemById(ItemID id)
    {
        var vanilla = DatabaseInfoProvider.Items.ItemInfos.get_Item(id);
        if (vanilla == null)
        {
            var item = _items.FirstOrDefault(x => x.GetItemID == id);
            if (item != null)
                return item.Info;
        }
        return vanilla;
    }

    public CustomItemController GetItemControllerForID(ItemID id)
    {
        return itemControllers[id];
    }

    public List<CustomItemController> GetItemControllers()
    {
        return itemControllers.Values.ToList();
    }

    private void RegisterItems()
    {
        Plugin.Log.LogInfo($"Registering Items");
        var items = DatabaseInfoProvider.Items.ItemInfos;
        var triads = DatabaseInfoProvider.Items.TriadsInfos;
        foreach (var gkItem in this._items)
        {
            if (gkItem.Info.itemType == ItemType.Triad)
            {
                gkItem.SetupTriad();
                triads.Add(gkItem.GetItemID, gkItem.Info);
                Plugin.Log.LogInfo($"Registered Triad {gkItem.GetItemID}");
            }
            items.Add(gkItem.GetItemID, gkItem.Info);
            Plugin.Log.LogInfo($"Registered Item {gkItem.Name} {gkItem.GetItemID} {gkItem.Info.itemType}");
            LangAPI.Instance.AddItemLang(gkItem);
        }
    }
}