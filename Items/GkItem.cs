using System.Reflection;
using Cpp2IL.Core.Extensions;
using Gatekeeper.General.Currency;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Items;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using Il2CppInterop.Runtime;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace GKAPI.Items;

public class GkItem
{
    public string Description { get; private set; }
    public string Name { get; private set; }
    public string StatsDescription { get; private set; }
    
    public ItemID GetItemID => Info.ItemID;
    public ItemDatabaseInfo Info { get; private set; }
    private List<ItemID> triadItemIds = new();

    public void SetupTriad()
    {
        if (triadItemIds.Count != 3)
        {
            Plugin.Log.LogError($"Triad has incorrect number of items: {this.triadItemIds.Count}");
            return;
        }
        foreach (var itemID in triadItemIds)
        {
            var item = ItemAPI.Instance.GetItemById(itemID);
            if (item != null)
                this.Info.triadItems.Add(item);
            else
            {
                Plugin.Log.LogError($"Triad Item {itemID} not found for item {this.Name}. Skipping.");
                break;
            }
        }
        
    }

    public class Builder
    {
        private string id;
        private string name;
        private string description;
        private string statsDescription;
        private ItemType itemType = ItemType.Modifiers;
        private bool unlocked = false;
        private bool hidden = true;
        private int itemCost = 70;
        private int maxCount = -1;
        private ItemDropSource dropSource = ItemDropSource.All;
        private List<ParamModification> modifications = new();
        private List<ItemID> triadItems = new();

        private static string FormatTerm(ItemType itemType, string id, string suffix)
        {
            var prefix = itemType switch
            {
                ItemType.Modifiers => $"ITEM.BASE.{id}.{suffix}",
                ItemType.StructureChangers => $"ITEM.PURPLE.{id}.{suffix}",
                ItemType.Amulets => $"ITEM.YELLOW.{id}.{suffix}",
                ItemType.RunesOfCreation => $"ITEM.BLUE.{id}.{suffix}",
                ItemType.CursedSignatures => $"ITEM.RED.{id}.{suffix}",
                ItemType.Triad => $"TRIAD.{suffix}.{id}",
                _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
            };
            return prefix;
        }

        public Builder(string name, string description = "Empty description", string statsDescription = "Empty stats description")
        {
            this.id = name.ToUpper();
            this.name = name;
            this.description = description;
            this.statsDescription = statsDescription;
        }

        public Builder AsTriad(System.Collections.Generic.List<ItemID> itemIds)
        {
            foreach (var itemID in itemIds)
            {
                this.triadItems.Add(itemID);
            }
            return this.WithItemType(ItemType.Triad).WithDropSource((ItemDropSource)0-255).WithMaxCount(1).WithItemCost(500);
        }

        public Builder WithId(string id)
        {
            this.id = id;
            return this;
        }

        public Builder WithItemType(ItemType itemType)
        {
            this.itemType = itemType;
            return this;
        }

        public Builder SetUnlocked(bool unlocked)
        {
            this.unlocked = unlocked;
            return this;
        }

        public Builder SetHidden(bool hidden)
        {
            this.hidden = hidden;
            return this;
        }

        public Builder WithItemCost(int cost)
        {
            this.itemCost = cost;
            return this;
        }

        public Builder WithMaxCount(int maxCount)
        {
            this.maxCount = maxCount;
            return this;
        }

        public Builder WithDropSource(ItemDropSource dropSource)
        {
            this.dropSource = dropSource;
            return this;
        }

        public Builder AddModification(ItemParamModificationType modificationType, float baseValue, float levelValue)
        {
            var modification = new ParamModification()
            {
                modificationType = modificationType,
                firstLevelModificationValue = baseValue,
                otherLevelModificationValue = levelValue
            };
            modifications.Add(modification);
            return this;
        }

        public GkItem Build(int itemId)
        {
            Plugin.Log.LogInfo($"Building GkItem with id: {itemId}");
            var vanillaItems = DatabaseInfoProvider.Items.ItemInfos;
            var info = ScriptableObject.CreateInstance<ItemDatabaseInfo>();
            info.ItemID = (ItemID)itemId;
            //TODO add custom meshrenderer
            info.ItemFbx = vanillaItems.get_Item(ItemID.Fuse).ItemFbx;
            info.HideInDatabase = false;
            //TODO add projectiles
            info.ProjectileInfos = new List<ItemProjectileInfo>();

            Sprite sprite;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GKAPI.Assets.testassetbundle"))
            {
                var testBundle = AssetBundle.LoadFromMemory(stream!.ReadBytes());
                var asset = testBundle.LoadAsset("assets/testbundle/textures/logo-50x50.png", Il2CppType.Of<Sprite>());
                sprite = asset.TryCast<Sprite>();
                testBundle.Unload(false);
            }
            Plugin.Log.LogMessage($"Is sprite null? {sprite == null}");
            
            info.itemIcon = sprite ?? vanillaItems.get_Item(ItemID.Cannonade).itemIcon;
            info.id = id;
            info.itemType = itemType;
            info.itemMaxCount = maxCount;
            info.unlocked = unlocked;
            info.wasSeen = !hidden;
            info.modificationParams = modifications;
            info.dropSource = dropSource;
            info.currency = CurrencyType.Prism;
            info.itemCost = itemCost;
            info.itemObeliskCost = itemCost;
            info.itemPedestalCost = itemCost;
            info.itemTriadCost = itemCost;
            info.itemTriadCostStep = 500;
            info.itemCostStep = 5;
            var scrapValue = itemType switch
            {
                ItemType.Modifiers => 10,
                ItemType.StructureChangers => 40,
                ItemType.Amulets => 20,
                _ => 0
            };
            info.itemSellingCost = scrapValue;
            info.itemCreateCost = scrapValue;
            //TODO custom cost
            info.itemUnlockCost = 1;
            if (itemType == ItemType.Triad)
            {
                info.itemUnlockCost = 0;
                info.itemObeliskCost = 0;
                info.itemPedestalCost = 0;
                info.itemTriadCost = 0;
                info.itemTriadCostStep = 0;
                info.itemCostStep = 500;
                info.itemNameKey = FormatTerm(itemType, id, "NAME");
                info.itemLongDescKey = "";
                info.itemStatsKey = FormatTerm(itemType, id, "DESC");
            }
            else
            {
                info.itemNameKey = FormatTerm(itemType, id, "NAME");
                info.itemLongDescKey = FormatTerm(itemType, id, "DESC");
                info.itemStatsKey = FormatTerm(itemType, id, "STATS");
            }
            
            info.triadItems = new List<ItemDatabaseInfo>();
            info.name = $"{(int)itemType + 1}_{id}";
            return new GkItem()
            {
                Info = info,
                Name = name,
                Description = description,
                StatsDescription = statsDescription,
                triadItemIds = triadItems,
            };
        }
    }
}