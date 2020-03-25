using UnityEngine;

public static class Vector3Directions
{
    public static Vector3 UpLeft { get { return new Vector3(-1, 1, 0); } }
    public static Vector3 UpRight { get { return new Vector3(1, 1, 0); } }
    public static Vector3 DownLeft { get { return new Vector3(-1, -1, 0); } }
    public static Vector3 DownRight { get { return new Vector3(1, -1, 0); } }
    public static Vector3 ForwardLeft { get { return new Vector3(-1, 0, 1); } }
    public static Vector3 ForwardRight { get { return new Vector3(1, 0, 1); } }
    public static Vector3 BackLeft { get { return new Vector3(-1, 0, -1); } }
    public static Vector3 BackRight { get { return new Vector3(1, 0, -1); } }
}
