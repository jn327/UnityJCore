using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public DebugOptionsSO debugOptions;

    private Gradient _goodBadColorGradient = new Gradient();
    private Color _defaultTextColor = new Color(0.9f, 0.9f, 0.9f);
    private float _debugUISpacing = 4;

    private float _paddingV = 0.01f;
    private float _paddingH = 0.01f;

    private int _currentFPS = 0;
    private int _targetFPS = 60;
    private List<int> _fpsLog = new List<int>();
    private int _fpsLogLength = 30;
    private Vector2 _fpsLogSize = new Vector2(200, 50);


    private List<DebugLogEvent> _logStack = new List<DebugLogEvent>();
    private Vector2 _logScrollPosition = Vector2.zero;

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

    private void Update()
    {
        //fps logging
        _currentFPS = (int)(1.0f / Time.smoothDeltaTime);
        _fpsLog.Add(_currentFPS);

        while (_fpsLog.Count > _fpsLogLength)
        {
            _fpsLog.RemoveAt(0);
        }
    }

    public int currentFPS
    {
        get { return _currentFPS; }
    }

    private void OnGUI()
    {
        float paddingL = _paddingH * Screen.width;
        float paddingT = _paddingV * Screen.height;
        Rect screenRect = new Rect(paddingL, paddingT, Screen.width - (paddingL * 2), Screen.height - (paddingT * 2));
        Rect itemRect = screenRect;
        itemRect.width = 100;
        itemRect.height = 22;

        GUI.color = _defaultTextColor;
        debugOptions.showFPS = GUI.Toggle(itemRect, debugOptions.showFPS, "Show fps");
        
        itemRect.x += itemRect.width + _debugUISpacing;
        debugOptions.showLog = GUI.Toggle(itemRect, debugOptions.showLog, "Show log");
        itemRect.x = screenRect.x;
        itemRect.y += itemRect.height + _debugUISpacing;

        if (debugOptions.showFPS)
        {
            float fpsNormal = 1 - ((float)_currentFPS / (float)_targetFPS);
            GUI.color = _goodBadColorGradient.Evaluate(fpsNormal);
            GUI.Label(itemRect, _currentFPS.ToString() + " FPS");
            itemRect.y += itemRect.height + _debugUISpacing;

            Color rectColor;
            int currFPS;
            Rect rect = new Rect(itemRect.x, itemRect.y, _fpsLogSize.x / _fpsLog.Count, 1);
            for (int i = 0; i < _fpsLog.Count; i++)
            {
                currFPS = _fpsLog[i];
                fpsNormal = 1 - ((float)currFPS / (float)_targetFPS);
                rectColor = _goodBadColorGradient.Evaluate(fpsNormal);

                rect.y = itemRect.y + (_fpsLogSize.y * fpsNormal) - (rect.height * 0.5f);
                
                GUIEx.DrawRect(rect, rectColor);
                rect.x += rect.width;
            }
            itemRect.y += _fpsLogSize.y + _debugUISpacing;
        }

        if (debugOptions.showLog)
        {
            GUI.color = _defaultTextColor;
            GUI.Label(itemRect, "Log:");

            itemRect.x += itemRect.width + _debugUISpacing;
            if (GUI.Button(itemRect, "Add to log"))
            {
                _logStack.Add(new DebugLogEvent("Manual click log", Color.yellow));
            }
            itemRect.x = screenRect.x;

            itemRect.y += itemRect.height + _debugUISpacing;
            
            //scroll view for the log
            Rect rect = new Rect(0, 0, 10, itemRect.height);
            int spacing = 2;
            Rect windowRect = new Rect(itemRect.x, itemRect.y, 200, itemRect.height + 16);
            Rect contentRect = new Rect(0, 0, (rect.width + spacing) * _logStack.Count, rect.height);
            _logScrollPosition = GUI.BeginScrollView(windowRect, _logScrollPosition, contentRect);

            for (int i = 0; i < _logStack.Count; i++)
            {
                GUIEx.DrawRect(rect, _logStack[i].color);
                rect.x += rect.width + spacing;
            }

            // End the scroll view that we began above.
            GUI.EndScrollView();

            itemRect.y += windowRect.height + _debugUISpacing; 
        }
    }
}
