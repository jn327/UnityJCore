using UnityEngine;

[System.Serializable]
public class Vector2MinMaxSet
{
    public Vector2 min;
    public Vector2 max;
    public Vector2MinMaxSet(Vector2 _min, Vector2 _max)
    {
        min = _min;
        max = _max;
    }
    
    public Vector2 Lerp(float t)
    {
        return new Vector2 
        (
            getLerpedX(t),
            getLerpedY(t)
        );
    }

    public Vector2 Lerp(Vector2 t)
    {
        return new Vector2 
        (
            getLerpedX(t.x),
            getLerpedY(t.y)
        );
    }

    public float getLerpedX(float t)
    {
        return Mathf.Lerp(min.x, max.x, t);
    }
    public float getLerpedY(float t)
    {
        return Mathf.Lerp(min.y, max.y, t);
    }

    public Vector2 InverseLerp(float t)
    {
        return new Vector2 
        (
            getInverseLerpedX(t),
            getInverseLerpedY(t)
        );
    }

    public Vector2 InverseLerp(Vector2 t)
    {
        return new Vector2 
        (
            getInverseLerpedX(t.x),
            getInverseLerpedY(t.y)
        );
    }

    public float getInverseLerpedX(float t)
    {
        return Mathf.InverseLerp(min.x, max.x, t);
    }
    public float getInverseLerpedY(float t)
    {
        return Mathf.InverseLerp(min.y, max.y, t);
    }
}