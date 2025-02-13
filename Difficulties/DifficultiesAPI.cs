using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using Gatekeeper.CameraScripts.HUD.StatisticPanel;
using Gatekeeper.General;
using Gatekeeper.General.Predictor;
using Gatekeeper.MainMenuScripts.MainMenu.CharacterSelectPanel;
using Gatekeeper.Other.UiSelectableStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GKAPI.Difficulties;

[HarmonyPatch]
public class DifficultiesAPI
{
    private int _nextId = System.Enum.GetValues<GameDifficulty>().Length;
    private readonly Dictionary<GameDifficulty, GkDifficulty> _gkDifficulties = [];
    private Sprite _backgroundSprite;
    private Sprite _diamondSprite;
    
    public static DifficultiesAPI Instance { get; } = new();

    private DifficultiesAPI()
    {
        EventHandler.Init += RegisterDifficulties;
        EventHandler.LateInit += CollectGameAssets;
    }

    public void AddDifficulty(float difficultyMultiplier, float prismMultiplier)
    {
        AddDifficulty(new GkDifficulty.Builder().WithDifficultyMultiplier(difficultyMultiplier).WithPrismMultiplier(prismMultiplier));
    }

    public void AddDifficulty(GkDifficulty.Builder builder)
    {
        var difficulty = builder.Build();
        var gameDifficulty = (GameDifficulty)_nextId++;
        _gkDifficulties.Add(gameDifficulty, difficulty);
    }

    private void RegisterDifficulties()
    {
        Plugin.Log.LogInfo("Registering difficulties");
        foreach (var (gameDifficulty, difficulty) in _gkDifficulties)
        {
            var difficultyValues = new DifficultyValues()
            {
                difficulty = gameDifficulty,
                difficultyMultiplier = difficulty.DifficultyMultiplier,
                prismMultiplier = difficulty.PrismMultiplier,
                StringPercentage = difficulty.PercentageName,
            };
            GameDesignValuesHolder.Instance.DifficultyValues.Add(gameDifficulty, difficultyValues);
            GameDesignValuesHolder.Instance.GameEventsDifficultyMinLevels.Add(gameDifficulty, difficulty.EventsMinLevel);
        }
    }

    private void CollectGameAssets()
    {
        if (this._backgroundSprite != null)
            return;
        Plugin.Log.LogInfo("Collecting game assets");
        foreach (var sprite in Resources.FindObjectsOfTypeAll<Sprite>())
        {
            switch (sprite.name)
            {
                case "Checkmark_2":
                    this._diamondSprite = sprite;
                    break;
                case "button_line2_polygon5_13":
                    this._backgroundSprite = sprite;
                    break;
            }
        }
        Plugin.Log.LogInfo($"Collected game assets, background: {_backgroundSprite == null}, diamond: {_diamondSprite == null}");
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
            __instance.toggleGroup.AddToggle(diffUiToggle);
        }

        return;

        /*
        foreach (var (gameDifficulty, difficulty) in Instance._gkDifficulties)
        {
            var diffObject = new GameObject($"Toggle - {difficulty.DifficultyName}");
            var diffToggleDifficulty = diffObject.AddComponent<ToggleDifficulty>();
            diffToggleDifficulty.GameDifficulty = gameDifficulty;
            var diffUiToggle = diffObject.GetComponent<UIToggle>();
            //UnityAction<ToggleValueChangedEvent> onValueChanged = (e) => animator.OnValueChanged(e);
            //uiToggle.onToggleValueChangedCallback = onValueChanged;
            diffObject.AddComponent<CanvasGroup>();
            var diffSelectableAnimator = diffObject.AddComponent<UISelectableUIAnimator>();
            diffSelectableAnimator.Controller = diffUiToggle;
            var diffSelectableSender = diffObject.AddComponent<SelectableSender>();
            diffSelectableSender.selectable = diffUiToggle;
            
            var background = new GameObject("Background");
            background.transform.SetParent(diffObject.transform);
            var backgroundColorAnimator = background.AddComponent<UIToggleColorAnimator>();
            backgroundColorAnimator.Controller = diffUiToggle;
            
            var backgroundColor = new Color(0.1f, 0.1f, 0.5f);
            Plugin.Log.LogInfo($"t1");
            var backgroundColorAnimation = new ColorAnimation();
            backgroundColorAnimation.startColor = backgroundColor;
            backgroundColorAnimation.Animation.Enabled = true;
            backgroundColorAnimation.RegisterCallbacks();
            var backgroundColorTarget = background.AddComponent<ImageColorTarget>();
            Plugin.Log.LogInfo($"t2");
            backgroundColorTarget.SetColor(backgroundColor);
            backgroundColorAnimation.colorTarget = backgroundColorTarget;
			
            backgroundColorAnimator.OnAnimation = backgroundColorAnimation;
            backgroundColorAnimator.OffAnimation = backgroundColorAnimation;
            backgroundColorAnimator.ColorTarget = backgroundColorTarget;
            Plugin.Log.LogInfo($"t3");
            var backgroundUIAnimator = background.AddComponent<UIToggleUIAnimator>();
            backgroundUIAnimator.Controller = diffUiToggle;
            Plugin.Log.LogInfo($"t4");
            //background.AddComponent<CanvasGroup>();
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.preserveAspect = true;
            backgroundImage.sprite = Instance._backgroundSprite;
            backgroundColorTarget.Target = backgroundImage;
            
            Plugin.Log.LogInfo($"Sprite is null? {backgroundImage.sprite == null}");
            
            var checkmark = new GameObject("Checkmark");
            var checkmarkColorAnimator = checkmark.AddComponent<UIToggleColorAnimator>();
            checkmarkColorAnimator.Controller = diffUiToggle;
            
            var checkmarkColor = new Color(0.1f, 0.1f, 0.5f);
            var checkmarkColorAnimation = new ColorAnimation
            {
                startColor = checkmarkColor,
            };
            var checkmarkColorTarget = checkmark.AddComponent<ImageColorTarget>();
            checkmarkColorTarget.color = checkmarkColor;
            
            checkmarkColorAnimator.ColorTarget = checkmarkColorTarget;
            checkmarkColorAnimator.OnAnimation = checkmarkColorAnimation;
            checkmarkColorAnimator.OffAnimation = checkmarkColorAnimation;
            checkmark.transform.SetParent(background.transform);
            var checkmarkImage = checkmark.GetComponent<Image>();
            checkmarkImage.sprite = Instance._diamondSprite;
            checkmarkColorTarget.Target = checkmarkImage;
            
            var controllerTransform = __instance.gameObject.transform.GetChild(1);
            diffObject.transform.SetParent(controllerTransform);
            __instance._toggles.Add(diffToggleDifficulty);
            __instance.toggleGroup.AddToggle(diffUiToggle);
        }*/
    }
    
    [HarmonyPatch(typeof(StatisticPanelView), nameof(StatisticPanelView.FillDifficulty))]
    [HarmonyPrefix]
    private static bool PatchFillDifficulty(ref StatisticPanelView __instance)
    {
        Plugin.Log.LogMessage("Filling difficulty");
        var difficulty = GlobalPredictor.Difficulty;

        __instance.difficultText.m_text = difficulty switch
        {
            GameDifficulty.Easy => "Passer: 80%",
            GameDifficulty.Medium => "Observer: 100%",
            GameDifficulty.Hard => "Participant: 150%",
            GameDifficulty.Insane => "Gatekeeper: 200%",
            _ => !Instance._gkDifficulties.TryGetValue(difficulty, out var gkDifficulty) ? "Any: any%" : $"{gkDifficulty.DifficultyName}: {gkDifficulty.PercentageName}",
        };
        return false;
    }
}