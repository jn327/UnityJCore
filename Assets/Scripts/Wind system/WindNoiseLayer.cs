using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WindNoiseLayer
{
    [SerializeField]
    protected string _name; //just for the inspector.
    private enum NOISE_TYPE {SIMPLEX, PERLIN, NONE, RANDOM, X, Y, SINX, SINY, COSX, COSY };
    [SerializeField]
    private NOISE_TYPE _type = default;

    [SerializeField]
    private AnimationCurve _curve = default;

    public enum OPERATION_TYPE {ADDITION, SUBTRACTION, MULTIPLICATION, DIVISION };
    [SerializeField]
	private OPERATION_TYPE _operationType = default;

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

    public bool IsEnabled ()
    {
        return _bEnabled;
    }

    public float GetValueAtPos( float currVal, float x, float y )
    {
        float val = 0;
        switch(_type)
        {
            case NOISE_TYPE.SIMPLEX:
                val = SimplexNoise.Noise( (_initialOffset.x + x) * _scale, (_initialOffset.y + y) * _scale, Time.time * _changeSpeed );
                break;
            case NOISE_TYPE.PERLIN:
                val = Perlin.Noise( (_initialOffset.x + x) * _scale, (_initialOffset.y + y) * _scale, Time.time * _changeSpeed );
                break;
            case NOISE_TYPE.RANDOM:
                val = Random.value;
                break;  
            case NOISE_TYPE.NONE:
                val = 1;
                break;
            case NOISE_TYPE.X:
                val = x / _scale;
                break;
            case NOISE_TYPE.Y:
                val = y / _scale;
                break;
            case NOISE_TYPE.SINX:
                val = Mathf.Sin(x * _scale);
                break;
            case NOISE_TYPE.SINY:
                val = Mathf.Sin(y * _scale);
                break;
            case NOISE_TYPE.COSX:
                val = Mathf.Cos(x * _scale);
                break;
            case NOISE_TYPE.COSY:
                val = Mathf.Cos(y * _scale);
                break;
        }

        if (_curve.keys.Length > 0)
        {
            val = _curve.Evaluate(val);
        }

        val *= _weighting;

        switch(_operationType)
        {
            case OPERATION_TYPE.ADDITION:
                currVal += val;
                break;
            case OPERATION_TYPE.DIVISION:
                currVal /= val;
                break;
            case OPERATION_TYPE.MULTIPLICATION:
                currVal *= val;
                break;
            case OPERATION_TYPE.SUBTRACTION:
                currVal -= val;
                break;
        }

        return currVal;
    }
}
