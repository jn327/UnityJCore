using UnityEngine;

[System.Serializable]
public class MinMaxFloat
{
    public float min;
    public float max;

    public MinMaxFloat(float _min, float _max)
    {
        min = _min;
        max = _max;
    }

    public float Lerp( float t )
    {
        return Mathf.Lerp( min, max, t );
    }

    public float InverseLerp( float value )
    {
        return Mathf.InverseLerp( min, max, value );
    }

    public float GetRandom()
    {
        return Mathf.Lerp( min, max, Random.value );
    }

    public float ClampValue( float value )
    {
        return Mathf.Clamp( value, min, max );
    }
}
