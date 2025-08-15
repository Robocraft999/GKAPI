using System.Collections.Generic;
using System.Linq;
using Gatekeeper.Achievements.Data;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Items;

namespace GKAPI.Achievements;

public class AchievementsAPI
{
    public static AchievementsAPI Instance { get; } = new();
    private List<GkAchievement> _achievements = new();
    private static List<ushort> _existingIndicies = System.Enum.GetValues<AchievementID>().Cast<ushort>().ToList();
    private static int _nextAchievementIndex = 5;
    private static int NextAchievementIndex
    {
        get 
        {
            while (_existingIndicies.Contains((ushort)_nextAchievementIndex)) _nextAchievementIndex++;
            return _nextAchievementIndex++;
        }
    }

    private AchievementsAPI()
    {
        EventHandler.Init += RegisterAchievements;
    }
    
    public GkAchievement AddAchievement(GkAchievement.Builder builder){
        var achievement = builder.Build(NextAchievementIndex);
        _achievements.Add(achievement);
        return achievement;
    }

    private void RegisterAchievements()
    {
        Plugin.Log.LogInfo($"Registering Achievements");
        var achievements = DatabaseInfoProvider.StoreAchievements.StoreAchievementInfos;
        foreach (var achievement in _achievements)
        {
            achievement.WriteItems();
            achievements.Add(achievement.Info);
        }
        Plugin.Log.LogInfo($"Registered {_achievements.Count} Achievements");
    }
}