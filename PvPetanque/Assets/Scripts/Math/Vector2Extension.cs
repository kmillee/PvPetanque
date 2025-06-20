using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 Exp(this Vector2 v)
    {
        return new Vector2(Mathf.Exp(v.x), Mathf.Exp(v.y));
    }

    public static Vector2 Sigmoid(this Vector2 v)
    {
        return new Vector2(
            1f / (1f + Mathf.Exp(-v.x)),
            1f / (1f + Mathf.Exp(-v.y))
        );
    }

    public static Vector2 Sigmoid(this Vector2 v, float k = 1f, float x0 = 0f)
    {
        return new Vector2(
            1f / (1f + Mathf.Exp(-k * (v.x - x0))),
            1f / (1f + Mathf.Exp(-k * (v.y - x0)))
        );
    }
}