using System;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;

[Serializable] public class NegatableDecisionsList : MMReorderableArray<NegatableDecision>{}
[Serializable] public class NegatableDecision
{
    public bool Negate;
    public AIDecision Decision;
    public bool Decide() => Negate ? !Decision.Decide() : Decision.Decide();
}
[AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionComposite")]
public class AIDecisionComposite : AIDecision
{
    [Tooltip("Returns true when any decision returns true")] public bool Any;
    [MMReorderableAttribute(null, "Decisions", null)] public NegatableDecisionsList Decisions;
    public override bool Decide() => Any ? Decisions.Any(decision => decision.Decide()) : Decisions.All(decision => decision.Decide());
}
