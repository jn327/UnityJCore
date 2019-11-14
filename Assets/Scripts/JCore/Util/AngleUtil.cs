using UnityEngine;

public class AngleUtil
{
    public static float ClampAngle(float angle, float min, float max)
    {
        //first we need to put it in a -360 to 360 range
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

    //normalize the angle to a -180 to 180 range
    public static float Normalize(float angle)
    {
        angle = angle % 360; 

        if (angle > 180)  
            angle -= 360;
        
        return angle;
    }
}
