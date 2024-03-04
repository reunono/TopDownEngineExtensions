using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PlayerDamageMultiplier))]
public class PlayerDamageMultiplierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var playerDamageMultiplier = (PlayerDamageMultiplier)target;
        if (DrawDefaultInspector()) Player.DamageMultiplier = playerDamageMultiplier.Value;
        else if (Application.isPlaying) playerDamageMultiplier.Value = Player.DamageMultiplier;
    }
}
#endif

public static class Player
{
    public static float DamageMultiplier = 1;
}

public class PlayerDamageMultiplier : MonoBehaviour, MMEventListener<MMDamageTakenEvent>
{
    public float Value = Player.DamageMultiplier;
    public void OnMMEvent(MMDamageTakenEvent damageTakenEvent)
    {
        var instigator = damageTakenEvent.Instigator;
        if (!instigator) return;
        Character owner;
        if (instigator.TryGetComponent<DamageOnTouch>(out var damage) && damage.Owner && damage.Owner.TryGetComponent<Character>(out var character)) owner = character;
        else if (instigator.TryGetComponent<Weapon>(out var weapon)) owner = weapon.Owner;
        else return;
        if (owner.CharacterType != Character.CharacterTypes.Player) return;
        var newDamage = Player.DamageMultiplier * damageTakenEvent.DamageCaused;
        var health = damageTakenEvent.AffectedHealth;
        health.SetHealth(damageTakenEvent.PreviousHealth - newDamage);
        health.LastDamage = newDamage;
    }
    private void Awake() => Player.DamageMultiplier = Value;
    private void OnEnable() => this.MMEventStartListening();
    private void OnDisable() => this.MMEventStopListening();
    public void Set(float multiplier) => Player.DamageMultiplier = multiplier;
    public void Add(float multiplier) => Player.DamageMultiplier += multiplier;
    public void Multiply(float multiplier) => Player.DamageMultiplier *= multiplier;
}
