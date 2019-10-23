using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public DebugOptionsSO debugOptions;

    private Gradient _goodBadColorGradient = new Gradient();
    private Color _defaultTextColor = Color.white;
    private float _debugUISpacing = 4;
    private float _debugScrollBarHeight = 16;

    private float _paddingV = 0.01f;
    private float _paddingH = 0.01f;

    private int _currentFPS = 0;
    private int _targetFPS = 60;
    private List<int> _fpsLog = new List<int>();
    private int _fpsLogLength = 30;
    private Vector2 _fpsLogSize = new Vector2(200, 50);


    private List<DebugLogEvent> _logs = new List<DebugLogEvent>();
    private Vector2 _logScrollPosition = Vector2.zero;
    private int _selectedLogIndex = 0;
    private Vector2 _selectedLogScrollPosition = Vector2.zero;
    private List<DebugLogEvent> _recentLogs = new List<DebugLogEvent>();
    private int _maxRecentLogs = 3;

    [System.Serializable]
    public struct DebugLogEvent 
    {
        private float _time;
        public float time { get { return _time; } }
        private string _message;
        public string message { get { return _message; } }
        private UnityEngine.Color _color;
        public UnityEngine.Color color { get { return _color; } }
        private string _stackTrace;
        public string stackTrace {get { return _stackTrace; } }
        private UnityEngine.Texture2D _screenshot;
        public UnityEngine.Texture2D screenshot { get { return _screenshot; } }

        // =======================
        //		Constructor
        // =======================
        public DebugLogEvent( string message, UnityEngine.Color color )
        {
            _time		= UnityEngine.Time.time;
            _stackTrace	= UnityEngine.StackTraceUtility.ExtractStackTrace ();
            _message	= message;
            _color		= color;

            _screenshot	= UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();

            UnityEngine.Debug.Log("<color=#"+UnityEngine.ColorUtility.ToHtmlStringRGB(_color)+">" +_time +": " +_message+ "</color>");
        }
    }

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

    public void addLogEvent( string message, Color color )
    {
        DebugLogEvent newLog = new DebugLogEvent(message, color);
        addLogEvent(newLog);
    }

    public void addLogEvent( string message )
    {
        DebugLogEvent newLog = new DebugLogEvent(message, Color.gray);
        addLogEvent(newLog);
    }

    private void addLogEvent( DebugLogEvent logEvent )
    {
        _logs.Add(logEvent);

        if (_selectedLogIndex == _logs.Count - 2)
        {
            _selectedLogIndex++;
        }

        _recentLogs.Add(logEvent);
        while (_recentLogs.Count > _maxRecentLogs)
        {
            _recentLogs.RemoveAt(0);
        }
    }

    public void clearLog( )
    {
        _selectedLogIndex = 0;
        while (_logs.Count > 0)
        {
            _logs.RemoveAt(0);
        }
        while (_recentLogs.Count > 0)
        {
            _recentLogs.RemoveAt(0);
        }
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
            itemRect.width = 40;
            GUI.Label(itemRect, "Log:");

            itemRect.x += itemRect.width + _debugUISpacing;
            itemRect.width = 80;
            if (GUI.Button(itemRect, "Add to log"))
            {
                addLogEvent("Manual click log", Color.green);
            }
            itemRect.x += itemRect.width + _debugUISpacing;
            itemRect.width = 70;
            if (GUI.Button(itemRect, "Clear log"))
            {
                clearLog();
            }
            itemRect.x = screenRect.x;

            itemRect.y += itemRect.height + _debugUISpacing;
            
            //scroll view for the log
            Rect rect = new Rect(0, 0, 10, itemRect.height);
            int spacing = 2;
            Rect windowRect = new Rect(itemRect.x, itemRect.y, 200, itemRect.height + _debugScrollBarHeight);
            Rect contentRect = new Rect(0, 0, (rect.width + spacing) * _logs.Count, rect.height);
            _logScrollPosition = GUI.BeginScrollView(windowRect, _logScrollPosition, contentRect);

            //only draw the ones that are within the visible scroll area...
            int startIndex = Mathf.Max(Mathf.FloorToInt(_logScrollPosition.x/(rect.width + spacing)), 0);
            int endIndex = Mathf.Min(startIndex + Mathf.CeilToInt(windowRect.width/(rect.width + spacing)) + 1, _logs.Count);
            
            Color backgroundColor = GUI.backgroundColor;
            for (int i = startIndex; i < endIndex; i++)
            {
                rect.x = i * (rect.width + spacing);

                GUI.backgroundColor = _logs[i].color;
                if (GUI.Button(rect, "", GUIEx.textureStyle))
                {
                    _selectedLogIndex = i;
                }
                
                GUI.DrawTexture(rect, _logs[i].screenshot, ScaleMode.ScaleToFit, true);

                if (_selectedLogIndex == i)
                {
                    GUIEx.DrawRect( new Rect(rect.x, rect.y, rect.width, 2), Color.white );
                    GUIEx.DrawRect( new Rect(rect.x, rect.y, 2, rect.height), Color.white );
                    GUIEx.DrawRect( new Rect(rect.x + rect.width - 2, rect.y, 2, rect.height), Color.white );
                    GUIEx.DrawRect( new Rect(rect.x, rect.y + rect.height - 2, rect.width, 2), Color.white );
                }
            }
            GUI.backgroundColor = backgroundColor;

            // End the scroll view that we began above.
            GUI.EndScrollView();

            itemRect.y += windowRect.height + _debugUISpacing; 

            //selected log
            if (_logs.Count > 0)
            {
                DebugLogEvent selLogEvent = _logs[_selectedLogIndex];

                Rect selectedLogWindowRect = new Rect(itemRect.x, itemRect.y, 200, 150);
                Rect selectedLogContentRect = new Rect(0, 0, 200 - _debugScrollBarHeight, 300 + 70 + _debugUISpacing + _debugScrollBarHeight);
                _selectedLogScrollPosition = GUI.BeginScrollView(selectedLogWindowRect, _selectedLogScrollPosition, selectedLogContentRect);
                
                GUI.color = Color.white;
                rect.width = 70;
                rect.height = 70;
                rect.x = 0; 
                rect.y = 0;

                GUI.DrawTexture(rect, selLogEvent.screenshot, ScaleMode.ScaleToFit, true);
                rect.x += rect.width + _debugUISpacing; 
                rect.width = 100;

                GUI.color = _defaultTextColor;
                GUI.Label(rect, selLogEvent.time +": " +selLogEvent.message);
                rect.y += rect.height + _debugUISpacing; 
                rect.x = screenRect.x;

                rect.width = 160;
                rect.height = 200;
                GUI.Label(rect, selLogEvent.stackTrace);
                rect.y += rect.height + _debugUISpacing; 

                GUI.EndScrollView();
                itemRect.y += selectedLogWindowRect.height + _debugUISpacing; 
            }
            
            //recent logs
            itemRect.width = 200;
            itemRect.height = 22;
            float eventLifetime = 2.0f;
            DebugLogEvent logEvent;
            for (int j = 0; j < _recentLogs.Count; j++)
            {
                logEvent = _recentLogs[j];
                GUI.color = logEvent.color;
                GUI.Label(itemRect, logEvent.time +": " +logEvent.message);

                if (Time.time - logEvent.time > eventLifetime )
                {
                    _recentLogs.RemoveAt(j);
                    j--;
                }

                itemRect.y += itemRect.height + _debugUISpacing; 
            }
            GUI.color = _defaultTextColor;

        }
    }
}
