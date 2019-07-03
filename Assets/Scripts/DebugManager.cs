using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public DebugOptionsSO debugOptions;

    private Gradient _goodBadColorGradient = new Gradient();

    private float _paddingV = 0.01f;
    private float _paddingH = 0.01f;

    private int _targetFPS = 60;

    private void Start()
    {
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0] = new GradientColorKey(Color.green, 0.0f);
        colorKeys[1] = new GradientColorKey(Color.yellow, 1.0f);
        colorKeys[2] = new GradientColorKey(Color.red, 1.0f);

        _goodBadColorGradient.SetKeys(colorKeys, alphaKeys);
    }

    private void OnGUI()
    {
        float paddingL = _paddingH * Screen.width;
        float paddingT = _paddingV * Screen.height;
        Rect screenRect = new Rect(paddingL, paddingT, Screen.width - (paddingL * 2), Screen.height - (paddingT * 2));

        if (debugOptions.showFPS)
        {
            int currFPS = (int)(1.0f / Time.smoothDeltaTime);
            GUI.color = _goodBadColorGradient.Evaluate(1 - ((float)currFPS / (float)_targetFPS));
            GUI.Label(screenRect, currFPS.ToString() + " FPS");
        }
    }
}
