using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [AddComponentMenu("TopDown Engine/Managers/Extended Input Manager")]
    public class ExtendedInputManager : InputManager
    {
        public MMInput.IMButton ProneButton { get; protected set; }
        protected override void InitializeButtons()
        {
            base.InitializeButtons();
            ButtonList.Add(ProneButton  = new MMInput.IMButton (PlayerID, "Prone"));
        }
    }
}
