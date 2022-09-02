using System.Linq;
using MoreMountains.Tools;
using UnityEngine;

public class AIDecisionPlayerIsInSameCinemachineZone : AIDecision
{
    private MMCinemachineZone2D[] _myZones;

    public override void Initialization()
    {
        base.Initialization();

        _myZones = FindObjectsOfType<MMCinemachineZone2D>().Where(zone => zone.GetComponent<BoxCollider2D>()
            .bounds.Contains(transform.position.MMSetZ(zone.transform.position.z))).ToArray();
    }

    public override bool Decide()
    {
        return _myZones.Any(room => room.VirtualCamera.enabled);
    }
}