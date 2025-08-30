using System.Reflection;
using Cpp2IL.Core.Extensions;
using Il2CppInterop.Runtime;
using UnityEngine;

namespace GKAPI.AssetBundles;

public static class AssetHelper
{
    public const string PlaceholderBundlePath = "GKAPI.Assets.testassetbundle";
    public const string PlaceholderSpritePath = "assets/testbundle/textures/logo-50x50.png";
    public const string PlaceholderPrefabPath = "assets/testbundle/Item_Placeholder.prefab";
    
    public static T LoadAsset<T>(string bundlePath, string assetPath) where T : UnityEngine.Object
    {
        Plugin.Log.LogInfo($"Loading asset {assetPath} in bundle {bundlePath} from assembly {Assembly.GetCallingAssembly().GetName().Name}");
        using var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(bundlePath);
        var assetBundle = AssetBundle.LoadFromMemory(stream!.ReadBytes());
        foreach (var an in assetBundle.AllAssetNames())
        {
            Plugin.Log.LogInfo($"   Contains {an}");
        }
        var rawAsset = assetBundle.LoadAsset(assetPath, Il2CppType.Of<T>());
        if (rawAsset == null)
        {
            Plugin.Log.LogWarning("Failed to load raw asset: " + assetPath + " in bundle: " +  bundlePath);
            return null;
        }
        var asset = rawAsset.TryCast<T>();
        assetBundle.Unload(false);
        if (asset == null)
            Plugin.Log.LogWarning("Failed to load asset: " + assetPath + " in bundle: " +  bundlePath);
        return asset;
    }
}