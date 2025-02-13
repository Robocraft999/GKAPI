using System;
using System.Collections.Generic;

namespace GKAPI;

public class PluginManager
{
    private static List<GkPlugin> plugins = [];

    public static void AddPlugin(GkPlugin plugin)
    {
        plugins.Add(plugin);
    }

    internal static void LoadPlugins()
    {
        /*var pluginss =
            from a in AppDomain.CurrentDomain.GetAssemblies()
            from t in a.GetTypes()
            let attribute = t.GetCustomAttribute(typeof(GkPluginA), true)
            where attribute != null
            select new { Type = t, Attribute = attribute };
        foreach (var plugin in pluginss)
        {
            
        }*/
        foreach (var plugin in plugins)
        {
            plugin.AddContent();
        }
        EventHandler.OnLoad();
    }

    private class GkPluginA : Attribute
    {
        
    }
}