using Gatekeeper.General.Currency;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Items;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using GKAPI.AssetBundles;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace GKAPI.Items;

public class GkItem
{
    public string Description { get; private init; }
    public string Name { get; private init; }
    public string StatsDescription { get; private init; }
    
    public ItemID GetItemID => Info.ItemId;
    public ItemDatabaseInfo Info { get; private init; }
    private List<ItemID> _triadItemIds = new();

    private const int ModifierCost = 70;
    private const int ModifierPedestalCost = 200;
    private const int ModifierScrapValue = 10;
    
    private const int StructureChangersCost = 140;
    private const int StructureChangersPedestalCost = 400;
    private const int StructureChangersScrapValue = 30;
    
    private const int AmuletCost = 100;
    private const int AmuletPedestalCost = 300;
    private const int AmuletScrapValue = 20;
    
    private const int CursedSignaturesCost = 0;
    private const int CursedSignaturesPedestalCost = 1;
    private const int CursedSignaturesScrapValue = 5;
    
    private const int RunesOfCreationCost = 210;
    private const int RunesOfCreationPedestalCost = 600;
    private const int RunesOfCreationScrapValue = 40;

    private const int TriadCost = 500;
    private const int TriadPedestalCost = 0;
    private const int TriadScrapValue = 0;

    public void SetupTriad()
    {
        if (_triadItemIds.Count != 3)
        {
            Plugin.Log.LogError($"Triad has incorrect number of items: {this._triadItemIds.Count}");
            return;
        }
        foreach (var itemID in _triadItemIds)
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
        private string _id;
        private string _name;
        private string _description;
        private string _statsDescription;
        private ItemType _itemType = ItemType.Modifiers;
        private bool _unlocked = false;
        private bool _hidden = true;
        private int _itemCost = ModifierCost;
        private int _itemPedestalCost = ModifierPedestalCost;
        private int _scrapValue = ModifierScrapValue;
        private int _maxCount = -1;
        private ItemDropSource _dropSource = ItemDropSource.All;
        private List<ParamModification> _modifications = new();
        private List<ItemID> _triadItems = new();
        private Sprite _icon = null;
        private GameObject _fbx = null;

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
            _id = name.ToUpper();
            _name = name;
            _description = description;
            _statsDescription = statsDescription;
        }

        public Builder AsModifier()
        {
            return WithItemType(ItemType.Modifiers).WithItemCost(ModifierCost).WithItemPedestalCost(ModifierPedestalCost).WithScrapValue(ModifierScrapValue);
        }
        
        public Builder AsStructureChanger()
        {
            return WithItemType(ItemType.StructureChangers).WithItemCost(StructureChangersCost).WithItemPedestalCost(StructureChangersPedestalCost).WithScrapValue(StructureChangersScrapValue);
        }
        
        public Builder AsAmulet()
        {
            return WithItemType(ItemType.Amulets).WithItemCost(AmuletCost).WithItemPedestalCost(AmuletPedestalCost).WithScrapValue(AmuletScrapValue);
        }
        
        public Builder AsCursedSignature()
        {
            return WithItemType(ItemType.CursedSignatures).WithItemCost(CursedSignaturesCost).WithItemPedestalCost(CursedSignaturesPedestalCost).WithScrapValue(CursedSignaturesScrapValue);
        }
        
        public Builder AsRuneOfCreation()
        {
            return WithItemType(ItemType.RunesOfCreation).WithItemCost(RunesOfCreationCost).WithItemPedestalCost(RunesOfCreationPedestalCost).WithScrapValue(RunesOfCreationScrapValue);
        }
        
        public Builder AsTriad(System.Collections.Generic.List<ItemID> itemIds)
        {
            foreach (var itemID in itemIds)
            {
                _triadItems.Add(itemID);
            }
            return WithItemType(ItemType.Triad).WithDropSource(ItemDropSource.None).WithMaxCount(1).WithItemCost(TriadCost).WithItemPedestalCost(TriadPedestalCost).WithScrapValue(TriadScrapValue);
        }

        public Builder WithId(string id)
        {
            _id = id;
            return this;
        }

        public Builder WithItemType(ItemType itemType)
        {
            _itemType = itemType;
            return this;
        }

        public Builder SetUnlocked(bool unlocked)
        {
            _unlocked = unlocked;
            return this;
        }

        public Builder SetHidden(bool hidden)
        {
            _hidden = hidden;
            return this;
        }

        public Builder WithIcon(Sprite icon)
        {
            _icon =  icon;
            return this;
        }

        public Builder WithModel(GameObject fbxPrefab)
        {
            _fbx = fbxPrefab;
            return this;
        }

        public Builder WithItemCost(int cost)
        {
            _itemCost = cost;
            return this;
        }

        public Builder WithItemPedestalCost(int cost)
        {
            _itemPedestalCost = cost;
            return this;
        }

        public Builder WithMaxCount(int maxCount)
        {
            _maxCount = maxCount;
            return this;
        }

        public Builder WithScrapValue(int scrapValue)
        {
            _scrapValue = scrapValue;
            return this;
        }

        public Builder WithDropSource(ItemDropSource dropSource)
        {
            _dropSource = dropSource;
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
            _modifications.Add(modification);
            return this;
        }

        public GkItem Build(int itemId)
        {
            Plugin.Log.LogInfo($"Building GkItem with id: {itemId}");
            var vanillaItems = DatabaseInfoProvider.Items.ItemInfos;
            var info = ScriptableObject.CreateInstance<ItemDatabaseInfo>();
            info.ItemId = (ItemID)itemId;

            MeshRenderer fbx = null;
            var vanillaMeshRendererMaterials = vanillaItems.get_Item(ItemID.Fuse).ItemFbx.materials;
            if (_fbx != null)
            {
                fbx = _fbx.GetComponent<MeshRenderer>();
            }
            else
            {
                var placeholderFbx = AssetHelper.LoadAsset<GameObject>(AssetHelper.PlaceholderBundlePath, AssetHelper.PlaceholderPrefabPath);
                if (placeholderFbx != null)
                    fbx = placeholderFbx.GetComponent<MeshRenderer>();
            }
            if (fbx != null)
                fbx.materials = vanillaMeshRendererMaterials;
            info.ItemFbx = fbx ?? vanillaItems.get_Item(ItemID.Fuse).ItemFbx;
            
            var itemIcon = _icon ?? AssetHelper.LoadAsset<Sprite>(AssetHelper.PlaceholderBundlePath, AssetHelper.PlaceholderSpritePath);
            info.DatabaseIcon = itemIcon ?? vanillaItems.get_Item(ItemID.Cannonade).DatabaseIcon;
            
            info.hideInDatabase = false;
            //TODO add projectiles
            info.ProjectileInfos = new List<ItemProjectileInfo>();
            
            info.Id = _id;
            info.itemType = _itemType;
            info.itemMaxCount = _maxCount;
            info.unlocked = _unlocked;
            info.wasSeen = !_hidden;
            info.modificationParams = _modifications;
            info.defaultDropSource = _dropSource;
            //TODO add separate function
            info.arenaDropSource = _dropSource;
            info.currency = CurrencyType.Prism;
            info.itemCost = _itemCost;
            info.itemCostStep = 0;
            info.itemObeliskCost = 0;
            info.itemPedestalCost = _itemPedestalCost;
            info.itemTriadCost = 0;
            info.itemTriadCostStep = 0;
            info.itemSellingCost = _scrapValue;
            info.itemCreateCost = _scrapValue;
            //TODO custom cost
            info.itemUnlockCost = 1;
            info.itemExcludeCost = 3;
            if (_itemType == ItemType.Triad)
            {
                info.itemUnlockCost = 0;
                info.itemCostStep = 500;
                info.itemNameKey = FormatTerm(_itemType, _id, "NAME");
                info.itemLongDescKey = "";
                info.itemStatsKey = FormatTerm(_itemType, _id, "DESC");
            }
            else
            {
                info.itemNameKey = FormatTerm(_itemType, _id, "NAME");
                info.itemLongDescKey = FormatTerm(_itemType, _id, "DESC");
                info.itemStatsKey = FormatTerm(_itemType, _id, "STATS");
            }
            
            info.triadItems = new List<ItemDatabaseInfo>();
            info.name = $"{(int)_itemType + 1}_{_id}";
            return new GkItem()
            {
                Info = info,
                Name = _name,
                Description = _description,
                StatsDescription = _statsDescription,
                _triadItemIds = _triadItems,
            };
        }
    }
}