using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Components;
using Gatekeeper.CameraScripts.HUD.Difficulty;
using Gatekeeper.CameraScripts.HUD.StatisticPanel;
using Gatekeeper.EnvironmentStuff.InstabilityCapsule;
using Gatekeeper.General;
using Gatekeeper.General.Extensions;
using Gatekeeper.General.Predictor;
using Gatekeeper.General.Tasks;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.MainMenuScripts.MainMenu.CharacterSelectPanel;
using GKAPI.Lang;
using HarmonyLib;
using RNGNeeds;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GKAPI.Difficulties;

[HarmonyPatch]
public class DifficultiesAPI
{
    private int _nextId = (int)GameDifficulty.Insane * 2;
    private readonly Dictionary<GameDifficulty, GkDifficulty> _gkDifficulties = [];
    
    public static DifficultiesAPI Instance { get; } = new();

    private DifficultiesAPI()
    {
        EventHandler.Init += RegisterDifficulties;
    }

    public (GameDifficulty, GkDifficulty) AddDifficulty(GkDifficulty.Builder builder)
    {
        var difficulty = builder.Build();
        var gameDifficulty = (GameDifficulty)_nextId;
        _nextId *= 2;
        _gkDifficulties.Add(gameDifficulty, difficulty);
        return (gameDifficulty, difficulty);
    }

    private void RegisterDifficulties()
    {
        Plugin.Log.LogInfo("Registering difficulties");
        foreach (var (gameDifficulty, difficulty) in _gkDifficulties)
        {
            GameDesignValuesHolder.Instance.difficultyValues.Add(gameDifficulty, difficulty.DifficultyValues);
            GameDesignValuesHolder.Instance.gameEventsData.Add(gameDifficulty, difficulty.GameEventData);
            GameDesignValuesHolder.Instance.defaultModeSirenSpawnData.Add(gameDifficulty, difficulty.SirenSpawnData);
            
            GameDesignValuesHolder.Instance.EnemyValues.EnemyExpLoopPow.Add(gameDifficulty, difficulty.EnemyExpLoopPow);
            GameDesignValuesHolder.Instance.EnemyValues.EnemyExpPointsPerEvo.Add(gameDifficulty, difficulty.EnemyExpPointsPerEvo);
            GameDesignValuesHolder.Instance.EnemyValues.EnemyExpPointsPerLoop.Add(gameDifficulty, difficulty.EnemyExpPointsPerLoop);
            GameDesignValuesHolder.Instance.EnemyValues.EnemyExpPointsPerTimeTick.Add(gameDifficulty, difficulty.EnemyExpPointsPerTimeTick);
            
            GameDesignValuesHolder.Instance.ArenaValues.gameEventsData.Add(gameDifficulty, difficulty.GameEventData);
            GameDesignValuesHolder.Instance.ArenaValues.EnemyExpRoundPow.Add(gameDifficulty, difficulty.ArenaValue.EnemyExpRoundPow);
            GameDesignValuesHolder.Instance.ArenaValues.EnemyExpPointsPerRound.Add(gameDifficulty, difficulty.ArenaValue.EnemyExpPointsPerRound);
            GameDesignValuesHolder.Instance.ArenaValues.EnemyPowerDifficultyCoefficient.Add(gameDifficulty, difficulty.ArenaValue.EnemyPowerDifficultyCoefficient);
            GameDesignValuesHolder.Instance.ArenaValues.FixedSirenDifficultyCoefficient.Add(gameDifficulty, difficulty.ArenaValue.FixedSirenDifficultyCoefficient);
            GameDesignValuesHolder.Instance.ArenaValues.SirenSpawnDifficultyModifier.Add(gameDifficulty, difficulty.ArenaValue.SirenSpawnDifficultyModifier);
            GameDesignValuesHolder.Instance.ArenaValues.ArenaExpPointsPerTime.Add(gameDifficulty, difficulty.ArenaValue.ArenaExpPointsPerTime);
            
            GameDesignValuesHolder.Instance.ElitesValues.arenaModeSpawnData.Add(gameDifficulty, difficulty.ElitesData);
            GameDesignValuesHolder.Instance.ElitesValues.defaultModeSpawnData.Add(gameDifficulty, difficulty.ElitesData);

            var maybeInstabilityCapsuleTask = DatabaseInfoProvider.Tasks._tasks.get_Item(TaskType.InstabilityCapsule).TryCast<InstabilityCapsuleTaskInfo>();
            if (maybeInstabilityCapsuleTask != null)
                maybeInstabilityCapsuleTask.capsulesSpeed.Add(gameDifficulty, difficulty.InstabilityCapsuleSpeed);
            
            LangAPI.Instance.AddDifficultyLang(difficulty);
        }
    }
    
    //probability for positive results
    public ProbabilityList<bool> CreateGameEventsProbabilities(float probability)
    {
        var probs = new ProbabilityList<bool>();
        probs.AddItem(new ProbabilityItem<bool>(true, probability));
        probs.AddItem(new ProbabilityItem<bool>(false, 1-probability));
        probs.PickValue();
        return probs;
    }

    [HarmonyPatch(typeof(PanelDifficulty), nameof(PanelDifficulty.Awake))]
    [HarmonyPostfix]
    private static void ConstructDifficultyPanel(ref PanelDifficulty __instance)
    {
        Plugin.Log.LogInfo("Adding UI patch");
        
        if (__instance == null)
        {
            Plugin.Log.LogError("PanelDifficulty instance is null");
            return;
        }

        var togglePrefab = Resources.FindObjectsOfTypeAll<UIToggle>().First(e => e.gameObject.name == "Toggle - Insane").gameObject;
        var diffInfo = __instance.difficultyInfo;

        foreach (var (gameDifficulty, difficulty) in Instance._gkDifficulties)
        {
            var controllerTransform = __instance.gameObject.transform.GetChild(1);
            var extraToggle = Object.Instantiate(togglePrefab, controllerTransform);
            extraToggle.name = $"Toggle - {difficulty.DifficultyName}";
            
            var diffToggleDifficulty = extraToggle.GetComponent<ToggleDifficulty>();
            diffToggleDifficulty.GameDifficulty = gameDifficulty;
            var diffUiToggle = extraToggle.GetComponent<UIToggle>();

            var background = extraToggle.transform.GetChild(0).gameObject;
            var backgroundColorTarget = background.GetComponent<ImageColorTarget>();
            backgroundColorTarget.color = difficulty.BackgroundColor;
            
            var checkmark = background.transform.GetChild(0).gameObject;
            var checkmarkColorTarget = checkmark.GetComponent<ImageColorTarget>();
            checkmarkColorTarget.color = difficulty.CheckmarkColor;
            
            __instance._toggles.Add(diffToggleDifficulty);
            __instance.difficultyToggles.AddItem(diffToggleDifficulty);
            __instance.toggleGroup.AddToggle(diffUiToggle);
            if (!diffInfo.BackgroundColors.ContainsKey(gameDifficulty))
                diffInfo.BackgroundColors.Add(gameDifficulty, difficulty.BackgroundColor);
            if (!diffInfo.CheckmarkColors.ContainsKey(gameDifficulty))
                diffInfo.CheckmarkColors.Add(gameDifficulty, difficulty.CheckmarkColor);
        }
    }
    
    private static GameDifficulty _cachedDifficulty;

    [HarmonyPatch(typeof(GameDifficultyIndicator), nameof(GameDifficultyIndicator.OnEnable))]
    [HarmonyPrefix]
    private static void PatchGameDifficultyIndicator(ref GameDifficultyIndicator __instance)
    {
        Plugin.Log.LogMessage("Filling difficulty - indicator");
        
        var diffInfo = __instance.difficultyInfo;
        foreach (var (gameDifficulty, difficulty) in Instance._gkDifficulties)
        {
            if (!diffInfo.BackgroundColors.ContainsKey(gameDifficulty))
                diffInfo.BackgroundColors.Add(gameDifficulty, difficulty.BackgroundColor);
            if (!diffInfo.CheckmarkColors.ContainsKey(gameDifficulty))
                diffInfo.CheckmarkColors.Add(gameDifficulty, difficulty.CheckmarkColor);
        }
        
    }

    [HarmonyPatch(typeof(StatisticPanelView), nameof(StatisticPanelView.FillInfo))]
    [HarmonyPrefix]
    private static void PrefixStatisticsPanel(ref StatisticPanelView __instance)
    {
        _cachedDifficulty = GlobalPredictor.Difficulty;
        GlobalPredictor.Difficulty = GameDifficulty.Insane;
    }
    
    [HarmonyPatch(typeof(StatisticPanelView), nameof(StatisticPanelView.FillInfo))]
    [HarmonyPostfix]
    private static void PostfixStatisticsPanel(ref StatisticPanelView __instance)
    {
        GlobalPredictor.Difficulty = _cachedDifficulty;
        var difficulty = _cachedDifficulty;
        Plugin.Log.LogMessage($"Filling difficulty - statistics ({_cachedDifficulty})");

        var key = "MENU.UI.DIFFICULTY." + difficulty switch
        {
            GameDifficulty.Medium => "OBSERVER",
            GameDifficulty.Hard => "PARTICIPANT",
            GameDifficulty.Insane => "GATEKEEPER",
            _ => !Instance._gkDifficulties.TryGetValue(difficulty, out var gkDifficulty) ? "OBSERVER" : gkDifficulty.TranslationKey
        };
        __instance.difficultText.m_text = Extensions.Translate(__instance, key);
    }
}