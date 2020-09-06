using System.Linq;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AIDecisionPlayerIsInSameRoom : AIDecision
{
    Room[] myRooms;

    public override void Initialization()
    {
        base.Initialization();

        myRooms = FindObjectsOfType<Room>().Where(room => room.GetComponent<BoxCollider2D>()
            .bounds.Contains(transform.position)).ToArray();
    }

    public override bool Decide()
    {
        if (_brain.Target == null && LevelManager.Instance != null)
        {
            _brain.Target = LevelManager.Instance.Players[0].transform;
        }

        return CheckIfTargetIsInSameRoom();
    }

    protected virtual bool CheckIfTargetIsInSameRoom()
    {
        return myRooms.Any(room => room.CurrentRoom);
    }
}
