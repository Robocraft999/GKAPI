using System.Collections.Generic;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Events;
using Gatekeeper.CameraScripts.HUD.StatisticPanel;
using Gatekeeper.General;
using Gatekeeper.General.Predictor;
using Gatekeeper.MainMenuScripts.MainMenu.CharacterSelectPanel;
using Gatekeeper.Other.UiSelectableStuff;
using HarmonyLib;
using Il2CppSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GKAPI;

[HarmonyPatch]
public static class UI_Patch
{
    static bool didInit = false;
    static int nextDiffId = System.Enum.GetValues<GameDifficulty>().Length;
    static List<DifficultyValues> Difficulties = [];

    public static void AddDifficulty(float difficultyMultiplier, float prismMultiplier, string percentageName)
    {
        var difficulty = (GameDifficulty)nextDiffId++;
        
        var difficultyValues = new DifficultyValues()
        {
            difficulty = difficulty,
            difficultyMultiplier = difficultyMultiplier,
            prismMultiplier = prismMultiplier,
            StringPercentage = percentageName,
        };
        
        Difficulties.Add(difficultyValues);
        
        GameDesignValuesHolder.Instance.DifficultyValues.Add(difficulty, difficultyValues);
        //TODO add custom class with more control like name etc
        GameDesignValuesHolder.Instance.GameEventsDifficultyMinLevels.Add(difficulty, 0);
    }
    
    /*[HarmonyPatch(typeof(PanelDifficulty), nameof(PanelDifficulty.Awake))]
    [HarmonyPostfix]*/
    static void AddUIPatch(ref PanelDifficulty __instance)
    {
        Plugin.Log.LogError("Adding UI patch");
        if (!didInit)
        {
            Plugin.Log.LogMessage("Didn't inited Difficulties yet");
            AddDifficulty(0.6f, 2f, "300%");
            
            didInit = true;
        }

        if (__instance == null)
        {
            Plugin.Log.LogError("PanelDifficulty instance is null");
            return;
        }

        foreach (var difficultyValue in Difficulties)
        {
            var difficulty = difficultyValue.difficulty;
            var diffObject = new GameObject($"Toggle - Custom{difficulty}");
            var toggleDifficulty = diffObject.AddComponent<ToggleDifficulty>();
            toggleDifficulty.GameDifficulty = difficulty;
            
            var background = new GameObject("Background");
            background.transform.SetParent(diffObject.transform);
            
            var checkmark = new GameObject("Checkmark");
            var animator = checkmark.AddComponent<UIToggleColorAnimator>();
            var colorTarget = checkmark.AddComponent<ImageColorTarget>();
            animator.ColorTarget = colorTarget;
            checkmark.transform.SetParent(background.transform);
            
            var uiToggle = diffObject.GetComponent<UIToggle>();
            //UnityAction<ToggleValueChangedEvent> onValueChanged = (e) => animator.OnValueChanged(e);
            //uiToggle.onToggleValueChangedCallback = onValueChanged;
            
            var canvasGroup = diffObject.AddComponent<CanvasGroup>();
            var selectableAnimator = diffObject.AddComponent<UISelectableUIAnimator>();
            selectableAnimator.Controller = uiToggle;
            
            var sender = diffObject.AddComponent<SelectableSender>();
            sender.selectable = uiToggle;
            
            var controllerTransform = __instance.gameObject.transform.GetChild(1);
            diffObject.transform.SetParent(controllerTransform);
            __instance._toggles.Add(toggleDifficulty);
            __instance.toggleGroup.AddToggle(uiToggle);
        }
    }

    [HarmonyPatch(typeof(PanelDifficulty), nameof(PanelDifficulty.UpdateDifficultyTextPercentage))]
    [HarmonyPrefix]
    static bool PatchUpdateDifficultyTextPercentage(ref PanelDifficulty __instance, GameDifficulty difficulty)
    {
        Plugin.Log.LogMessage($"Turn toggle on: {difficulty}");
        return true;
    }

    /*[HarmonyPatch(typeof(StatisticPanelView), nameof(StatisticPanelView.FillDifficulty))]
    [HarmonyPrefix]*/
    static bool PatchFillDifficulty(ref StatisticPanelView __instance)
    {
        Plugin.Log.LogMessage("Filling difficulty");
        var difficulty = GlobalPredictor.Difficulty;
        __instance.difficultText.m_text = difficulty switch
        {
            GameDifficulty.Easy => "Passer: 80%",
            GameDifficulty.Medium => "Observer: 100%",
            GameDifficulty.Hard => "Participant: 150%",
            GameDifficulty.Insane => "Gatekeeper: 200%",
            (GameDifficulty)4 => "Guardian: 300%",
            _ => "Unknown difficulty"
        };
        return false;
    }
}