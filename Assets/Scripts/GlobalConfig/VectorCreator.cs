using UnityEngine;

public class VectorCreator
{
    public static Vector2 SetVector2(float x = 0, float y = 0) => new Vector2(x, y);
    public static Vector3 SetVector3(float x = 0, float y = 0, float z = 0) => new Vector3(x, y, z);
}
