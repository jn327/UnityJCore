using UnityEngine;

public static class CurlNoiseUtil
{
    public delegate Vector2 NoiseFunctionDelegate(float x, float y);

    public static Vector2 CurlNoise(NoiseFunctionDelegate noiseFunction, float x, float y, float step = 1)
    {
        //larger step results in larger curls
        
        //rate of change in x
        float a = (noiseFunction(x + step, y).x - noiseFunction(x - step, y).x);
        //average it
        a /= (step * 2);

        //rate of change in y
        float b = (noiseFunction(x, y + step).y - noiseFunction(x, y - step).y);
        //average it
        b /= (step * 2);
        
        return new Vector2(b, -a).normalized;
        //return new Vector2(a, b).normalized;
        //return new Vector2(a, -b).normalized;
    }
}
