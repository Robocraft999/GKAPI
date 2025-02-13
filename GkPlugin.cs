using BepInEx.Unity.IL2CPP;

namespace GKAPI;

public abstract class GkPlugin : BasePlugin
{
    public abstract void AddContent();

    public override void Load()
    {
        //TODO maybe use attribute to do this automatically
        PluginManager.AddPlugin(this);
    }
}