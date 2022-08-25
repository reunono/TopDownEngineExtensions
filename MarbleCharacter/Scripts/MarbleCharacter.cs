using MoreMountains.TopDownEngine;

public class MarbleCharacter : Character
{
    protected override void Initialization()
    {
        base.Initialization();
        CharacterDimension = CharacterDimensions.Type3D;
    }
    
    public override void Freeze()
    {
        _controller.SetKinematic(true);
        ConditionState.ChangeState(CharacterStates.CharacterConditions.Frozen);
    }

    public override void UnFreeze()
    {
        _controller.SetKinematic(false);
        ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
    }
}
