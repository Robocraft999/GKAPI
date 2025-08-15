using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace GKAPI;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BasePlugin
{
    public const string PluginGuid = "com.robocraft999.gkapi";
    public const string PluginName = "GKAPI";
    private const string PluginVersion = "0.1.0";
    
    internal new static ManualLogSource Log;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;

        try
        {
            Log.LogMessage(" ");
            Log.LogMessage("Inserting Harmony Hooks...");
            
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.robocraft999.gkapi.il2cpp");

            Log.LogMessage("Runtime Hooks's Applied");
            Log.LogMessage(" ");
        }
        catch (Exception e)
        {
            Log.LogError($"FAILED to Apply Hooks's! {e.Message}");
        }
        
        Log.LogInfo($"Plugin {PluginName} is loaded!");

        IL2CPPChainloader.Instance.Finished += PluginManager.LoadPlugins;
    }
}
