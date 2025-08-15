using System;
using System.Linq;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Items;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using GKAPI.Lang;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;

namespace GKAPI.Items;

public class ItemAPI
{
    private readonly System.Collections.Generic.List<GkItem> _items = [];
    internal System.Collections.Generic.Dictionary<ItemID, Il2CppSystem.Type> itemControllerTypes = new();
    internal readonly System.Collections.Generic.Dictionary<ItemID, CustomItemController> itemControllers = new();

    private readonly System.Collections.Generic.List<int> _existingIds = System.Enum.GetValues<ItemID>().Cast<int>().ToList();
    private int _nextItemId = 0;
    private int NextItemId
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

    public GkItem AddTriad(string name, System.Collections.Generic.List<ItemID> itemIds, Func<GkItem.Builder, GkItem.Builder> build)
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
        if (vanilla != null) return vanilla;
        var item = _items.FirstOrDefault(x => x.GetItemID == id);
        return item?.Info;
    }

    public CustomItemController GetItemControllerForID(ItemID id)
    {
        return itemControllers[id];
    }

    public System.Collections.Generic.List<CustomItemController> GetItemControllers()
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
        
        Plugin.Log.LogInfo($"All Items");
        foreach (var e in items)
        {
            Plugin.Log.LogInfo($"   Item {e.value.ItemId} {e.value.itemType} {e.value.itemNameKey.mTerm} {e.value.itemLongDescKey.mTerm}");
        }
        Plugin.Log.LogInfo($"Registered {_items.Count} Items");
    }
}