using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TopDownEngineExtensions
{
    [AddComponentMenu("TopDown Engine/Managers/Multiple Weapons Input Manager")]
    public class MultipleWeaponsInputManager : InputManager
    {
        // how many weapons a single character can handle at once
        public int MaximumNumberOfWeapons = 3;
        public MMInput.IMButton[] ShootButtons;
        public MMInput.IMButton[] ReloadButtons;
        public MMInput.ButtonStates[] ShootAxes { get; protected set; }
        protected string[] _axesShoot;

        public void ShootButtonDown(int handleWeaponID) => ShootButtons[handleWeaponID - 1].State.ChangeState(MMInput.ButtonStates.ButtonDown);
        public void ShootButtonPressed(int handleWeaponID) => ShootButtons[handleWeaponID - 1].State.ChangeState(MMInput.ButtonStates.ButtonPressed);
        public void ShootButtonUp(int handleWeaponID) => ShootButtons[handleWeaponID - 1].State.ChangeState(MMInput.ButtonStates.ButtonUp);
        
        public void ReloadButtonDown(int handleWeaponID) => ReloadButtons[handleWeaponID - 1].State.ChangeState(MMInput.ButtonStates.ButtonDown);
        public void ReloadButtonPressed(int handleWeaponID) => ReloadButtons[handleWeaponID - 1].State.ChangeState(MMInput.ButtonStates.ButtonPressed);
        public void ReloadButtonUp(int handleWeaponID) => ReloadButtons[handleWeaponID - 1].State.ChangeState(MMInput.ButtonStates.ButtonUp);

        protected override void Awake()
        {
            ShootButtons = new MMInput.IMButton[MaximumNumberOfWeapons];
            ReloadButtons = new MMInput.IMButton[MaximumNumberOfWeapons];
            ShootAxes = new MMInput.ButtonStates[MaximumNumberOfWeapons];
            _axesShoot = new string[MaximumNumberOfWeapons];
            base.Awake();
        }

        protected override void InitializeButtons()
        {
            base.InitializeButtons();
            ShootButtons[0] = ShootButton;
            ReloadButtons[0] = ReloadButton;
            ShootButtons[1] = SecondaryShootButton;
            ButtonList.Add(ReloadButtons[1] = new MMInput.IMButton (PlayerID, "Reload2"));
            for (var i = 2; i < MaximumNumberOfWeapons; i++)
            {
                ButtonList.Add(ShootButtons[i] = new MMInput.IMButton(PlayerID, "Shoot" + (i + 1)));
                ButtonList.Add(ReloadButtons[i] = new MMInput.IMButton (PlayerID, "Reload" + (i + 1)));
            }
        }

        protected override void InitializeAxis()
        {
            base.InitializeAxis();
            _axesShoot[0] = _axisShoot;
            _axesShoot[1] = _axisShootSecondary;
            for (var i = 2; i < MaximumNumberOfWeapons; i++)
                _axesShoot[i] = PlayerID + "_ShootAxis" + (i + 1);
        }

        protected override void SetShootAxis()
        {
            base.SetShootAxis();
            if (IsMobile || !InputDetectionActive) return;
            ShootAxes[0] = ShootAxis;
            ShootAxes[1] = SecondaryShootAxis;
            for (var i = 2; i < MaximumNumberOfWeapons; i++)
                ShootAxes[i] = MMInput.ProcessAxisAsButton(_axesShoot[i], Threshold.y, ShootAxes[i]);
        }
    }
}
