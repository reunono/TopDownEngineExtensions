using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
public class SetBrainTargetToDamageInstigatorOwner : MonoBehaviour, MMEventListener<MMDamageTakenEvent>{
    public void OnMMEvent(MMDamageTakenEvent damageTakenEvent){
        var instigator = damageTakenEvent.Instigator;
        if (!instigator) return;
        Character owner;
        if (instigator.TryGetComponent<DamageOnTouch>(out var damage) && damage.Owner && damage.Owner.TryGetComponent<Character>(out var character)) owner = character;
        else if (instigator.TryGetComponent<Weapon>(out var weapon) && weapon.Owner) owner = weapon.Owner;
        else return;
        if (damageTakenEvent.AffectedHealth.TryGetComponent<Character>(out var affectedCharacter) && affectedCharacter.CharacterBrain)
            affectedCharacter.CharacterBrain.Target = owner.transform;
    }
    private void OnEnable() => this.MMEventStartListening();
    private void OnDisable() => this.MMEventStopListening();
}
