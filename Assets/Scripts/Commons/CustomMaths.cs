using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMaths : Object
{
    public static Vector2 Lerp(Vector2 v1, Vector2 v2, float t)
    {
        return new Vector2(
            Mathf.Lerp(v1.x, v2.x, t), 
            Mathf.Lerp(v1.y, v2.y, t)
            );
    }

    public static Vector3 Lerp(Vector3 v1, Vector3 v2, float t)
    {
        return new Vector3(
            Mathf.Lerp(v1.x, v2.x, t),
            Mathf.Lerp(v1.y, v2.y, t),
            Mathf.Lerp(v1.z, v2.z, t)
            );
    }

    public static Vector4 Lerp(Vector4 v1, Vector4 v2, float t)
    {
        return new Vector4(
            Mathf.Lerp(v1.x, v2.x, t),
            Mathf.Lerp(v1.y, v2.y, t),
            Mathf.Lerp(v1.z, v2.z, t),
            Mathf.Lerp(v1.w, v2.w, t)
            );
    }

    public static Vector2 Clamp(Vector2 v, float min, float max)
    {
        return new Vector2(Mathf.Clamp(v.x, min, max), Mathf.Clamp(v.y, min, max));
    }

    public static Vector2 Clamp(Vector2 v, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y));
    }

    public static Vector2 Clamp(Vector2 v, Vector2 extent)
    {
        return Clamp(v, -extent, extent);
    }
}
