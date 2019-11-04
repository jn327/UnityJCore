using UnityEngine;

public class AngleUtil
{
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle > 360)
        {
            angle = (angle % 360);
        }
        else if (angle < -360)
        {
            angle = (angle % -360);
        }

        return Mathf.Clamp(angle, min, max);
    }
}
