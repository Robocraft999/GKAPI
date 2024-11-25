using Gatekeeper.CameraScripts.HUD.SkillPanelStuff;
using Gatekeeper.General.Events.Characters;
using Gatekeeper.Items;
using GKAPI.Items;

namespace GKAPI.Content.ItemControllers;

public class TestTriadItemController : CustomItemController
{
    public override void ClientHandleSkillUsed(EventClientCharacterSkillUsed eventData)
    {
        if (eventData.SkillType == SkillType.Third)
        {
            Plugin.Log.LogInfo("TestTriadItemController::HandleThirdSkillUsed");
        }
    }
}