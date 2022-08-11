using MoreMountains.Tools;

public enum Perspectives { TopDown, FirstPerson }
public struct PerspectiveChangeEvent
{
    public Perspectives NewPerspective;
    static PerspectiveChangeEvent e;
    public static void Trigger(Perspectives newPerspective)
    {
        e.NewPerspective = newPerspective;
        MMEventManager.TriggerEvent(e);
    }
}
