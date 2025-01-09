using System.Collections.Generic;
using Gatekeeper.Achievements.Data;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using Gatekeeper.MainMenuScripts.MainMenu.StorePanel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace GKAPI.Achievements;

public class GkAchievement
{
    public StoreAchievementInfo Info { get; private set; }
    private List<ItemDatabaseInfo> items = [];

    public void writeItems()
    {
        Info.Items = new Il2CppReferenceArray<ItemDatabaseInfo>(items.ToArray());
    }

    public void AddItem(ItemDatabaseInfo item)
    {
        items.Add(item);
    }

    public void AddItems(ItemDatabaseInfo[] items)
    {
        this.items.AddRange(items);
    }
    
    public class Builder
    {
        public GkAchievement Build(int index, string name = "Test Achievement Name")
        {
            var achievement = new GkAchievement();
            var info = ScriptableObject.CreateInstance<StoreAchievementInfo>();
            info.name = name;
            info.Index = index;
            //TODO add option to change this
            info.AchievementID = AchievementID.None;
            achievement.Info = info;
            return achievement;
        }
    }
}