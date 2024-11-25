using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Gatekeeper.EnvironmentStuff.Obelisks;
using Gatekeeper.General.GameEvents;
using Gatekeeper.Items;
using Gatekeeper.MainMenuScripts.Database.ItemsDatabaseController;
using Gatekeeper.Infrastructure.Providers.InfoProviders;
using Gatekeeper.MainMenuScripts.MainMenu.MainMenuPanel;
using GKAPI.Content.ItemControllers;
using GKAPI.Difficulties;
using GKAPI.Items;
using GKAPI.Lang;
using HarmonyLib;
using I2.Loc;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMono;
using Il2CppSystem.Collections.Generic;
using Pathfinding.Collections;
using UnityEngine;

namespace GKAPI;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[HarmonyPatch]
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
            //ClassInjector.RegisterTypeInIl2Cpp<Bootstrapper>();
            //ClassInjector.RegisterTypeInIl2Cpp<TrainerComponent>();
        }
        catch
        {
            Log.LogError("FAILED to Register Il2Cpp Type!");
        }

        try
        {
            Log.LogMessage(" ");
            Log.LogMessage("Inserting Harmony Hooks...");

            //var harmony = new Harmony("robocraft999.gkapi.il2cpp");
            /*var originalUpdate = AccessTools.Method(typeof(UnityEngine.UI.CanvasScaler), "Update");
            Log.LogMessage("   Original Method: " + originalUpdate.DeclaringType.Name + "." + originalUpdate.Name);
            var postUpdate = AccessTools.Method(typeof(Test2.Bootstrapper), "Update");
            Log.LogMessage("   Postfix Method: " + postUpdate.DeclaringType.Name + "." + postUpdate.Name);
            harmony.Patch(originalUpdate, postfix: new HarmonyMethod(postUpdate));*/
            
            /*var originalUpdate = AccessTools.Method(typeof(Gatekeeper.MainMenuScripts.MainMenu.MainMenuPanel.MainMenuController), "Awake");
            Log.LogMessage("   Original Method: " + originalUpdate.DeclaringType.Name + "." + originalUpdate.Name);
            var postUpdate = AccessTools.Method(typeof(Plugin), "OnAwake");
            Log.LogMessage("   Postfix Method: " + postUpdate.DeclaringType.Name + "." + postUpdate.Name);
            harmony.Patch(originalUpdate, postfix: new HarmonyMethod(postUpdate));*/
            
            /*var originalStart = AccessTools.Method(typeof(Gatekeeper.MainMenuScripts.MainMenu.CharacterSelectPanel.PanelDifficulty), "Awake");
            Log.LogMessage("   Original Method: " + originalStart.DeclaringType.Name + "." + originalStart.Name);
            var postStart = AccessTools.Method(typeof(UI_Patch), "AddUIPatch");
            Log.LogMessage("   Postfix Method: " + postStart.DeclaringType.Name + "." + postStart.Name);
            harmony.Patch(originalStart, postfix: new HarmonyMethod(postStart));*/
            
            /*var originalOnToggle = AccessTools.Method(typeof(Gatekeeper.MainMenuScripts.MainMenu.CharacterSelectPanel.PanelDifficulty), "UpdateDifficultyTextPercentage");
            Log.LogMessage("   Original Method: " + originalOnToggle.DeclaringType.Name + "." + originalOnToggle.Name);
            var postOnToggle = AccessTools.Method(typeof(UI_Patch), "PatchUpdateDifficultyTextPercentage");
            Log.LogMessage("   Prefix Method: " + postOnToggle.DeclaringType.Name + "." + postOnToggle.Name);
            harmony.Patch(originalOnToggle, prefix: new HarmonyMethod(postOnToggle));*/
            
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.robocraft999.gkapi.il2cpp");

            Log.LogMessage("Runtime Hooks's Applied");
            Log.LogMessage(" ");
        }
        catch (Exception e)
        {
            Log.LogError($"FAILED to Apply Hooks's! {e.Message}");
        }

        AddContent();
        EventHandler.OnLoad();
        
        /*foreach (var pair in itemsInfoProvider.ItemInfos)
        {
            var info = pair.Value;
            Log.LogInfo(info.id);
            //info.itemCost = 10;
        }*/
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

    }

    private void AddContent()
    {
        if (EventHandler.State != EventHandler.LoadingState.PreInit)
        {
            Log.LogError("Content has to be added during Pre-Init!");
            return;
        }
        
        var itemAPI = ItemAPI.Instance;
        var testItem = itemAPI.AddItem(new GkItem.Builder("Test Item", "Test item description", $"{ColorHelper.WrapInColor("{[Mod1_Lvl1]}% (+{[Mod1_Lvl2]}% per stack)", Colors.Red)} to critical damage.")
            .WithId("TEST")
            .SetUnlocked(true)
            .SetHidden(false)
            .AddModification(ItemParamModificationType.CritDamagePerc, 0.5f, 0.25f)
        );
        itemAPI.AddItem("Bob");
        var testTriad = itemAPI.AddTriad("TestTriad", [ItemID.HVC, ItemID.StringOfEli, testItem.GetItemID], builder => builder.SetUnlocked(true).SetHidden(false));
        itemAPI.AddItemController<TestTriadItemController>(testTriad.GetItemID);

        var diffAPI = DifficultiesAPI.Instance;
        diffAPI.AddDifficulty(new GkDifficulty.Builder()
            .WithName("Guardian")
            .WithPercentageName("300%")
            .WithDifficultyMultiplier(0.6f)
            .WithPrismMultiplier(2f)
            .WithEventsMinLevel(0)
            .WithColors(new Color(0.1f, 0.1f, 0.5f), new Color(0.1f, 0.3f, 0.8f))
        );
        diffAPI.AddDifficulty(new GkDifficulty.Builder()
            .WithName("Wtf is this")
            .WithPercentageName("1000%")
            .WithDifficultyMultiplier(2f)
            .WithPrismMultiplier(4f)
            .WithEventsMinLevel(0)
            .WithColors(new Color(0.4f, 0.1f, 0.8f), new Color(0.4f, 0.3f, 0.8f))
        );
    }

    private static bool lateLoadCompleted;

    //[HarmonyPatch(typeof(MainMenuController), nameof(MainMenuController.Awake))]
    //[HarmonyPostfix]
    protected static void OnAwake()
    {
        if (lateLoadCompleted)
            return;
        
        Log.LogMessage("MainMenu Inited");
        
        var languageSource = new LanguageSourceData()
        {
            mLanguages = I2.Loc.LocalizationManager.Sources.get_Item(0).mLanguages,
        };
        //I2.Loc.LocalizationManager.AddSource(languageSource);
        
        //var asset = Resources.Load<TextAsset>( "localization.csv");
        //var content = GetResourceFileContentAsString(Paths.PluginPath "localization.csv");
        var content = "";
        
        try
        {
            // Open the text file using a stream reader.
            using StreamReader reader = new("localization.csv");

            // Read the stream as a string.
            content = reader.ReadToEnd();

            // Write the text to the console.
            Console.WriteLine(content);
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        // Check for errors (file not found)

        if (content == string.Empty)
        {
            Log.LogMessage("Unable to load Localization data");
            return;
        }
        
        //languageSource.Import_CSV(string.Empty, content);
        
        CreateTerm("ITEM.BASE.TEST.DESC", "Test item description");
        CreateTerm("ITEM.BASE.TEST.NAME", "Test item Name");
        CreateTerm("ITEM.BASE.TEST.STATS", "<color=#FF6A6A><b>{[Mod1_Lvl1]}% (+{[Mod1_Lvl2]}% per stack)</b></color> to critical damage.");

        //Seems to call an update function. Without this the terms don't get updated
        I2.Loc.LocalizationManager.GetTermsList();
        lateLoadCompleted = true;
    }
    
    public static string GetResourceFileContentAsString(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Test2.GKAPI." + fileName;

        string resource = null;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                resource = reader.ReadToEnd();
            }
        }
        return resource;
    }

    private static void CreateTerm(string key, string value)
    {
        var languages = new Il2CppStringArray([value, value, value, value, value, value, value, value, value, value, value, value, value]);
        var flags = new Il2CppStructArray<byte>([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        var termData = new TermData
        {
            Term = key,
            Description = "",
            TermType = eTermType.Text,
            Languages = languages,
            Flags = flags
        };
        /*var term = I2.Loc.LocalizationManager.Sources.get_Item(0).mTerms.get_Item(0);
        Log.LogMessage($"Term: {term.Term} {term.Description}");
        foreach(var lang in term.Languages)
        {
            Log.LogMessage($"Term lang: {lang}");
        }
        foreach(var lang in term.Flags)
        {
            Log.LogMessage($"Term Flags: {lang}");
        }*/

        //var languageSource = /*(I2.Loc.LanguageSourceAsset)*/ I2.Loc.ResourceManager.pInstance.mResourcesCache.get_Item("I2Languages");
        //var language = (ScriptableObject) I2.Loc.ResourceManager.pInstance.mResourcesCache.get_Item("I2Languages");
        /*LanguageSourceAsset l = new LanguageSourceAsset(I2.Loc.ResourceManager.pInstance.mResourcesCache.get_Item("I2Languages").GetCachedPtr());
        Log.LogInfo($"Resources: '{l}' ({l.GetType()}) ({typeof(I2.Loc.LanguageSourceAsset)})");*/
        
        /*foreach (var kv in I2.Loc.ResourceManager.mInstance.mResourcesCache)
        {
            Log.LogInfo($"Resources: '{kv.Key}' - {kv.Value} ({kv.Value.GetType()}) ({typeof(I2.Loc.LanguageSourceAsset)})");
            Log.LogInfo($"Resources:{kv.Value.name}");
            
        }*/
        
        I2.Loc.LocalizationManager.Sources.get_Item(0).mTerms.Add(termData);
    }
}
