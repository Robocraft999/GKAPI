using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using I2.Loc;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

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
            
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.robocraft999.gkapi.il2cpp");

            Log.LogMessage("Runtime Hooks's Applied");
            Log.LogMessage(" ");
        }
        catch (Exception e)
        {
            Log.LogError($"FAILED to Apply Hooks's! {e.Message}");
        }
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        IL2CPPChainloader.Instance.Finished += PluginManager.LoadPlugins;
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
