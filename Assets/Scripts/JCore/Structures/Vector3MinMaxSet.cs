using UnityEngine;

[System.Serializable]
public class Vector3MinMaxSet
{
    public Vector3 min;
    public Vector3 max;
    public Vector3MinMaxSet(Vector3 _min, Vector3 _max)
    {
        min = _min;
        max = _max;
    }
    
    public Vector3 Lerp(float t)
    {
        return new Vector3 
        (
            getLerpedX(t),
            getLerpedY(t),
            getLerpedZ(t)
        );
    }

    public Vector3 Lerp(Vector3 t)
    {
        return new Vector3 
        (
            getLerpedX(t.x),
            getLerpedY(t.y),
            getLerpedZ(t.z)
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

    public float getLerpedZ(float t)
    {
        return Mathf.Lerp(min.z, max.z, t);
    }

    public Vector3 InverseLerp(float t)
    {
        return new Vector3 
        (
            getInverseLerpedX(t),
            getInverseLerpedY(t),
            getInverseLerpedZ(t)
        );
    }

    public Vector3 InverseLerp(Vector3 t)
    {
        return new Vector3
        (
            getInverseLerpedX(t.x),
            getInverseLerpedY(t.y),
            getInverseLerpedZ(t.z)
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

    public float getInverseLerpedZ(float t)
    {
        return Mathf.InverseLerp(min.y, max.y, t);
    }
}