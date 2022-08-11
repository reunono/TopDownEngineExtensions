using UnityEngine;

public class SetLocalPositionZeroOnStart : MonoBehaviour
{
    private void Start()
    {
        transform.localPosition = Vector3.zero;
    }
}
