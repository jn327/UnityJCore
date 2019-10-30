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
}
