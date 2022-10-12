using MoreMountains.TopDownEngine;
using UnityEngine;

namespace TankControls
{
    public class CharacterRotation : MonoBehaviour
    {
        public float Speed = 5;
        private GameObject _model;

        private void Awake() => _model = GetComponentInParent<Character>().CharacterModel;

        private void Update() => _model.transform.Rotate(0.0f, Speed * Input.GetAxis("Horizontal"), 0.0f);
    }
}
