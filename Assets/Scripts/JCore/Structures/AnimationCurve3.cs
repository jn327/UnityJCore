using UnityEngine;

[System.Serializable]
public class AnimationCurve3
{
    public AnimationCurve x;
	public AnimationCurve y;
	public AnimationCurve z;
    public AnimationCurve3(AnimationCurve _x, AnimationCurve _y, AnimationCurve _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static AnimationCurve3 linear 
    { 
        get
        {
            return new AnimationCurve3
            (
                AnimationCurve.Linear(0, 0, 1, 1),
                AnimationCurve.Linear(0, 0, 1, 1),
                AnimationCurve.Linear(0, 0, 1, 1) 
            );
        }
    }

    public static AnimationCurve3 one 
    { 
        get
        {
            return new AnimationCurve3
            (
                AnimationCurve.Linear(0, 1, 1, 1),
                AnimationCurve.Linear(0, 1, 1, 1),
                AnimationCurve.Linear(0, 1, 1, 1) 
            );
        }
    }

    public Vector3 Evaluate3( Vector3 t )
    {
        if (x.keys.Length > 0)
        {
            t.x = x.Evaluate(t.x);
        }
        if (y.keys.Length > 0)
        {
		    t.y = y.Evaluate(t.y);
        }
        if (z.keys.Length > 0)
        {
		    t.z = z.Evaluate(t.z);
        }

        return t;
    }

    public Vector3 Evaluate3Random()
    {
        Vector3 t = Vector3.one;
        
        if (x.keys.Length > 0)
        {
            t.x = x.Evaluate(Random.value);
        }
        if (y.keys.Length > 0)
        {
            t.y = y.Evaluate(Random.value);
        }
        if (z.keys.Length > 0)
        {
            t.z = z.Evaluate(Random.value);
        }
        
        return t;
    }
}
