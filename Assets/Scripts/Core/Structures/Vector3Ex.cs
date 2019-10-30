using UnityEngine;

public class Vector3Ex
{
    public static Vector3 Lerp3( Vector3 a, Vector3 b, Vector3 t )
    {
        return new Vector3
        (
            Mathf.Lerp(a.x, b.x, t.x),
		    Mathf.Lerp(a.y, b.y, t.x),
		    Mathf.Lerp(a.z, b.z, t.z)
        );
    }

    public static Vector3 MoveTowards3( Vector3 current, Vector3 target, Vector3 maxDelta )
    {
        return new Vector3
        (
            Mathf.MoveTowards(current.x, target.x, maxDelta.x),
			Mathf.MoveTowards(current.y, target.y, maxDelta.y),
			Mathf.MoveTowards(current.z, target.z, maxDelta.z)
        );
    }

    public static Vector3 InverseLerp3(Vector3 a, Vector3 b, Vector3 value)
    {
        return new Vector3
        (
            Mathf.InverseLerp(a.x, b.x, value.x),
			Mathf.InverseLerp(a.y, b.y, value.y),
			Mathf.InverseLerp(a.z, b.z, value.z)
        );
    }

    public static Vector3 Distance3(Vector3 a, Vector3 b)
    {
        return new Vector3
        (
            Mathf.Abs(a.x - b.x),
			Mathf.Abs(a.y - b.y),
			Mathf.Abs(a.z - b.z)
        );
    }
}
