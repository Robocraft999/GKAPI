using System;
using Gatekeeper.General;
using Gatekeeper.MainMenuScripts.MainMenu.MainMenuPanel;
using HarmonyLib;

namespace GKAPI;

[HarmonyPatch]
public class EventHandler
{
    public static LoadingState State { get; private set; } = LoadingState.PreInit;
    public static Action Init;
    public static Action LateInit;
    public static Action StartGame;

    public static void OnLoad()
    {
        if (State == LoadingState.PreInit)
        {
            State = LoadingState.Init;
            Init?.Invoke();
        }
        else
            Plugin.Log.LogError("Invalid loading state");
    }

    [HarmonyPatch(typeof(MainMenuController), nameof(MainMenuController.Awake))]
    [HarmonyPostfix]
    private static void OnMenuLoad()
    {
        if (State != LoadingState.Init)
            return;
        
        State = LoadingState.LateInit;
        LateInit?.Invoke();
    }

    /*[HarmonyPatch(typeof(GameplayManager), nameof(GameplayManager.ClientInit))]
    [HarmonyPostfix]
    private static void OnGameStart()
    {
        StartGame?.Invoke();
    }*/

    public enum LoadingState
    {
        PreInit,
        Init,
        LateInit,
    }
}