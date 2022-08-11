using MoreMountains.Tools;
using UnityEngine;

public class PerspectiveCursorLock : MonoBehaviour, MMEventListener<PerspectiveChangeEvent>
{
    private void Awake()
    {
        this.MMEventStartListening();
    }

    private void OnDestroy()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(PerspectiveChangeEvent perspectiveChangeEvent)
    {
        Cursor.lockState = perspectiveChangeEvent.NewPerspective == Perspectives.FirstPerson ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
