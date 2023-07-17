using System.Collections;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;

public class ExtendedDialogueZone : DialogueZone
{
    [Header("Dialogue Actions")]
    public UnityEvent OnDialogueEnd;
    protected override IEnumerator PlayNextDialogue()
    {
        yield return base.PlayNextDialogue();
        if (_currentIndex == Dialogue.Length) OnDialogueEnd.Invoke();
    }
}
