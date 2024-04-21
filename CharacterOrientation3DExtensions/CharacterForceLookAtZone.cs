using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterForceLookAtZone : MonoBehaviour
{
    [SerializeField] private Transform Target;
    private void Awake()
    {
        if (TryGetComponent<Collider>(out var collider)) collider.isTrigger = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Character>(out var character)) character.FixedUpdateLookAt(Target);
    }
}
