using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WindNoiseLayer
{
    [SerializeField]
    private bool _bEnabled = true;

    //how quickly it changes over time
    [SerializeField]
    private float _changeSpeed = 0.01f;

    [SerializeField]
    [Range(0,1)]
    private float _weighting = 1.0f;
    
    //initial position offset, so we don't always get our noise from the same location
    private Vector2 _initialOffset;
    
    [SerializeField]
	private float _scale = 0.01f;

    public void Start ()
    {
        _initialOffset = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)) * 9999;
    }

    public float GetWeighting ()
    {
        return _weighting;
    }

    public bool IsEnabled ()
    {
        return _bEnabled;
    }

    public float GetValueAtPos( float x, float y )
    {
        //return Perlin.Noise( (_initialOffset.x + x) * _scale, (_initialOffset.y + y) * _scale, Time.time * _changeSpeed );
        return (1 + SimplexNoise.Noise( (_initialOffset.x + x) * _scale, (_initialOffset.y + y) * _scale, Time.time * _changeSpeed )) * 0.5f;
    }
}
