using UnityEngine;

[System.Serializable]
public class AnimationCurve2
{
    public AnimationCurve x;
	public AnimationCurve y;
    public AnimationCurve2(AnimationCurve _x, AnimationCurve _y)
    {
        x = _x;
        y = _y;
    }

    public static AnimationCurve2 linear 
    { 
        get
        {
            return new AnimationCurve2
            (
                AnimationCurve.Linear(0, 0, 1, 1),
                AnimationCurve.Linear(0, 0, 1, 1)
            );
        }
    }

    public static AnimationCurve2 one 
    { 
        get
        {
            return new AnimationCurve2
            (
                AnimationCurve.Linear(0, 1, 1, 1),
                AnimationCurve.Linear(0, 1, 1, 1)
            );
        }
    }

    public Vector2 Evaluate2( Vector2 t )
    {
        if (x.keys.Length > 0)
        {
            t.x = x.Evaluate(t.x);
        }
        if (y.keys.Length > 0)
        {
		    t.y = y.Evaluate(t.y);
        }

        return t;
    }

    public Vector2 Evaluate2Random()
    {
        Vector2 t = Vector2.one;
        
        if (x.keys.Length > 0)
        {
            t.x = x.Evaluate(t.x);
        }
        if (y.keys.Length > 0)
        {
            t.y = y.Evaluate(t.y);
        }
        
        return t;
    }
}
