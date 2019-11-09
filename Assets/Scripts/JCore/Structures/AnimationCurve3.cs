using UnityEngine;

[System.Serializable]
public class AnimationCurve3
{
    public AnimationCurve x;
	public AnimationCurve y;
	public AnimationCurve z;

    // ===== CONSTRUCTOR =====
    public AnimationCurve3(AnimationCurve _x, AnimationCurve _y, AnimationCurve _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    // ===== STATIC GETTERS =====
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

    // ===== METHODS =====
    public Vector3 Evaluate3( Vector3 t )
    {
        return new Vector3
        (
            x.Evaluate(t.x),
			y.Evaluate(t.y),
			z.Evaluate(t.z)
        );
    }

    public Vector3 Evaluate3Random()
    {
        return new Vector3
        (
            x.Evaluate(Random.value),
			y.Evaluate(Random.value),
			z.Evaluate(Random.value)
        );
    }
}
