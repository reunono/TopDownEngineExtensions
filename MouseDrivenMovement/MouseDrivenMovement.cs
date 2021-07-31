using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [RequireComponent(typeof(CharacterPathfinder3D), typeof(MouseDrivenPathfinderAI3D))]
    public class MouseDrivenMovement : CharacterMovement
    {
        public bool UseMouseDrivenMovement = true;

        protected override void Awake()
        {
            base.Awake();
            GetComponent<MouseDrivenPathfinderAI3D>().Destination =
                new GameObject(_character.PlayerID + "_Destination");
        }

        protected override void HandleInput()
        {
            if (!UseMouseDrivenMovement) base.HandleInput();
        }
    }
}