using MoreMountains.TopDownEngine;
using UnityEngine;

public class GridCharacterPathfinder3D : CharacterPathfinder3D
{
    private CharacterGridMovement _gridMovement;

    protected override void Awake()
    {
        base.Awake();
        _gridMovement = _character.FindAbility<CharacterGridMovement>();
    }

    protected override void MoveController()
    {
        if (Target == null || NextWaypointIndex <= 0)
            _gridMovement.SetMovement(Vector2.zero);
        else
        {
            _direction = Waypoints[NextWaypointIndex] - transform.position;
            if (Mathf.Abs(_direction.x) > Mathf.Abs(_direction.z))
            {
                _newMovement.x = _direction.x;
                _newMovement.y = 0;
            }
            else
            {
                _newMovement.x = 0;
                _newMovement.y = _direction.z;
            }
            _gridMovement.SetMovement(_newMovement);
        }
    }
}
