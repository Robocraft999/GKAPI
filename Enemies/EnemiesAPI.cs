using System.Collections.Generic;
using Gatekeeper.Enemy.Base.Data;
using Gatekeeper.Enemy.SpawnTemplates;
using Gatekeeper.General.Predictor;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.Utility.SceneHelper;
using RNGNeeds;
using UnityEngine;

namespace GKAPI.Enemies;

public class EnemiesAPI
{
    private readonly List<KnowingEnemySpawnTemplate> _templates = [];
    
    public static EnemiesAPI Instance { get; } = new();

    private EnemiesAPI()
    {
        EventHandler.Init += RegisterEnemies;
    }

    public KnowingEnemySpawnTemplate AddKnowingTemplate(GameDifficulty difficulties, bool forBossFight, List<SceneDesignation> scenes, Vector2Int locationNumberRange, ProbabilityList<EnemyCharacterInfo> enemyProbabilityList, string name = "", bool ignore = false)
    {
        var template = ScriptableObject.CreateInstance<KnowingEnemySpawnTemplate>();
        template.name = name;
        template.difficulties = difficulties;
        template.ForBossFight = forBossFight;
        template.Ignore = ignore;
        template.scenes = new Il2CppSystem.Collections.Generic.List<SceneDesignation>();
        foreach (var scene in scenes)
        {
            template.scenes.Add(scene);
        }
        template.locationNumberRange = locationNumberRange;
        template.ProbabilityListOfEnemy = enemyProbabilityList;
        _templates.Add(template);
        return template;
    }

    private void RegisterEnemies()
    {
        Plugin.Log.LogInfo("Registering enemies");

        foreach (var template in _templates)
        {
            DatabaseInfoProvider.Enemies.AllKnowingEnemySpawnTemplates.Add(template);
        }
        Plugin.Log.LogInfo($"Added {_templates.Count} knowing templates");
        
        Plugin.Log.LogInfo("Registered enemies");
    }
}