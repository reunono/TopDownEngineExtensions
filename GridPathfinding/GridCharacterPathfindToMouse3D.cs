using MoreMountains.TopDownEngine;
using UnityEngine;

public class GridCharacterPathfindToMouse3D : CharacterPathfindToMouse3D
{
    protected override void Initialization()
    {
        _abilityInitialized = true;
        _mainCamera = Camera.main;
        _characterPathfinder3D = gameObject.GetComponent<CharacterPathfinder3D>();
        _character.FindAbility<CharacterGridMovement>().InputMode = CharacterGridMovement.InputModes.Script;
            
        OnClickFeedbacks?.Initialization();
        _playerPlane = new Plane(Vector3.up, Vector3.zero);
        if (Destination != null) return;
        Destination = new GameObject();
        Destination.name = this.name + "PathfindToMouseDestination";
    }
}
