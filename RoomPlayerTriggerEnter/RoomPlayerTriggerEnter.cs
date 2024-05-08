using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class RoomPlayerTriggerEnter : MonoBehaviour
{
    [Tooltip("whether or not we should ask to move a MMSpriteMask on activation")]
    public bool MoveMask = true;
    [MMCondition("MoveMask", true)]
    [Tooltip("the curve to move the mask along to")]
    public MMTween.MMTweenCurve MoveMaskCurve = MMTween.MMTweenCurve.EaseInCubic;
    [MMCondition("MoveMask", true)]
    [Tooltip("the method used to move the mask")]
    public MMSpriteMaskEvent.MMSpriteMaskEventTypes MoveMaskMethod = MMSpriteMaskEvent.MMSpriteMaskEventTypes.ExpandAndMoveToNewPosition;
    [MMCondition("MoveMask", true)]
    [Tooltip("the duration of the mask movement (usually the same as the DelayBetweenFades")]
    public float MoveMaskDuration = 0.2f;
    private int _player;
    private Room _room;
    private void OnPlayerTriggerEnter()
    {
        _room.PlayerEntersRoom();
        if (_room.VirtualCamera) _room.VirtualCamera.Priority = 10;	
        if (MoveMask) MMSpriteMaskEvent.Trigger(MoveMaskMethod, _room.RoomColliderCenter, _room.RoomColliderSize, MoveMaskDuration, MoveMaskCurve);
    }
    private void Awake()
    {
        _player = LayerMask.NameToLayer("Player");
        _room = GetComponent<Room>();
    }
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.layer != _player) return;
        OnPlayerTriggerEnter();
    }
    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer != _player) return;
        OnPlayerTriggerEnter();
    }
}
