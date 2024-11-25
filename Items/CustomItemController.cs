using Gatekeeper.Enemy.Base;
using Gatekeeper.General.Events.Characters;
using Gatekeeper.General.Events.Enemies;
using Gatekeeper.General.Events.Items;
using Gatekeeper.Items;
using Gatekeeper.Utility;

namespace GKAPI.Items;

public abstract class CustomItemController : ItemController
{
    public virtual void ClientHandleSkillUsed(EventClientCharacterSkillUsed eventData){}
    public virtual void ClientHandleEnemyDied(EventClientEnemyDied eventData){}
    public virtual void ClientHandleInteractableUsed(EventClientInteractableObjectUsed eventData){}
    public virtual void OwnerHandleDamageTaken(DamageType damageType, float damageTaken){}
    public virtual void OwnerHandleCharacterHitSomething(EventOwnerCharacterHitSomething eventData){}
    public virtual void OwnerHandleFirstSkillHit(EnemyCharacterMain enemy, EventOwnerCharacterHitSomething eventData){}
    public virtual void OwnerSkillHit(EnemyCharacterMain enemy, EventOwnerCharacterHitSomething eventData){}
    public virtual void OwnerItemHit(EnemyCharacterMain enemy, EventOwnerCharacterHitSomething eventData){}
}