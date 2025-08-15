using Gatekeeper.Enemy.Base;
using Gatekeeper.General.Events.Characters;
using Gatekeeper.General.Events.Enemies;
using Gatekeeper.General.Events.Interactable;
using Gatekeeper.Items;
using Gatekeeper.Utility;

namespace GKAPI.Items;

public abstract class CustomItemController : ItemController
{
    public virtual void OwnerHandleSkillUsed(EventClientCharacterSkillUsed eventData){}
    public virtual void OwnerHandleOnEnemyDied(EventClientEnemyDied eventData){}
    public virtual void ClientHandleInteractableUsed(EventClientInteractableObjectUsed eventData){}
    public virtual void OwnerHandleDamageTaken(DamageType damageType, float damageTaken){}
    public virtual void OwnerHandleCharacterHitSomething(EventOwnerCharacterHitSomething eventData){}
    public virtual void OwnerHandleFirstSkillHit(IEnemy enemy, EventOwnerCharacterHitSomething eventData){}
    public virtual void OwnerSkillHit(IEnemy enemy, EventOwnerCharacterHitSomething eventData){}
    public virtual void OwnerItemHit(IEnemy enemy, EventOwnerCharacterHitSomething eventData){}
}