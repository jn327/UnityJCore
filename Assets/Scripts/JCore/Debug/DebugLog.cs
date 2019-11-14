using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
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
    private Vector2 _fpsLogSize = new Vector2(200, 30);

    private List<DebugLogEvent> _logs = new List<DebugLogEvent>();
     private int _logsLength = 500; //limit the amount of logs, they should not be being spammed.
    private Vector2 _logScrollPosition = Vector2.zero;
    private List<DebugLogEvent> _recentLogs = new List<DebugLogEvent>();
    private int _maxRecentLogs = 3;
    private List<DebugLogEvent> _logsAddedThisFrame = new List<DebugLogEvent>();

    [System.Serializable]
    public struct DebugLogEvent
    {
        private float _time;
        public float time { get { return _time; } }
        private string _message;
        public string message { get { return _message; } }
        private Color _color;
        public Color color { get { return _color; } }
        private string _stackTrace;
        public string stackTrace {get { return _stackTrace; } }
        public Texture2D _screenshot;
        public Texture2D screenshot {get { return _screenshot; } }

        // =======================
        //		Constructor
        // =======================
        public DebugLogEvent( string message, Color color )
        {
            _time		= UnityEngine.Time.time;
            _stackTrace	= StackTraceUtility.ExtractStackTrace ();
            _message	= message;
            _color		= color;

            _screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
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

    public static void add( string message, Color color )
    {
        DebugLogEvent newLog = new DebugLogEvent(message, color);
        GameManager.Instance.debugLog.addLogEvent(newLog);
    }

    public static void add( string message )
    {
        DebugLogEvent newLog = new DebugLogEvent(message, Color.gray);
        GameManager.Instance.debugLog.addLogEvent(newLog);
    }

    private void addLogEvent( DebugLogEvent logEvent )
    {
        _logs.Add(logEvent);
        while (_logs.Count > _logsLength)
        {
            _logs.RemoveAt(0);
        }

        _recentLogs.Add(logEvent);
        while (_recentLogs.Count > _maxRecentLogs)
        {
            _recentLogs.RemoveAt(0);
        }

        Debug.Log("<color=#"+ColorUtility.ToHtmlStringRGB(logEvent.color)+">" +logEvent.time +": " +logEvent.message+ "</color>");

        _logsAddedThisFrame.Add(logEvent);
        //if we've already added a log this frame then we only need to start the coroutine once
        if (_logsAddedThisFrame.Count <= 1)
        {
            StartCoroutine(LogScreen());
        }
    }

    // Will be called from camera after regular rendering is done.
    IEnumerator LogScreen()
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        DebugLogEvent logEvent;
        while (_logsAddedThisFrame.Count > 0)
        {
            logEvent = _logsAddedThisFrame[0];

            //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
            logEvent.screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            logEvent.screenshot.Apply();

            _logsAddedThisFrame.RemoveAt(0);
        }
    }

    public void clearLog( )
    {
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
        Event m_Event = Event.current;

        float paddingL = _paddingH * Screen.width;
        float paddingT = _paddingV * Screen.height;
        Rect screenRect = new Rect(paddingL, paddingT, Screen.width - (paddingL * 2), Screen.height - (paddingT * 2));
        Rect itemRect = screenRect;
        itemRect.width = 100;
        itemRect.height = 22;

        GUI.color = _defaultTextColor;
        DebugOptionsSO debugOptions = GameManager.Instance.debugOptions;
        //TODO: link this up to game manager
        debugOptions.showFPS = GUI.Toggle(itemRect, debugOptions.showFPS, debugOptions.showFPS ? "Pause" : "Un-Pause");
        itemRect.y += itemRect.height + _debugUISpacing;

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
            int hoverIndex = -1;
            for (int i = startIndex; i < endIndex; i++)
            {
                rect.x = i * (rect.width + spacing);

                GUIEx.DrawRect(rect, _logs[i].color );

                GUI.DrawTexture(rect, _logs[i].screenshot, ScaleMode.ScaleToFit, true);

                if (rect.Contains(m_Event.mousePosition))
                {
                    hoverIndex = i;
                }
            }

            // End the scroll view that we began above.
            GUI.EndScrollView();

            itemRect.y += windowRect.height + _debugUISpacing;

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

            //hovering over the logs
            if (hoverIndex > -1)
            {
                Rect hoverRect = new Rect(m_Event.mousePosition.x, m_Event.mousePosition.y, screenRect.width * 0.75f, screenRect.height * 0.75f);

                GUIEx.DrawRect( hoverRect, _logs[hoverIndex].color );

                float areaW = 250;
                float hoverRectW = hoverRect.width;
                hoverRect.x += 2;
                hoverRect.y += 2;
                hoverRect.height -= 4;
                hoverRect.width = Mathf.Max(hoverRect.width - (areaW + 4), 0);
                GUI.DrawTexture(hoverRect, _logs[hoverIndex].screenshot, ScaleMode.ScaleToFit, true);

                hoverRect.x -= 2;
                hoverRect.y -= 2;
                hoverRect.height += 4;
                hoverRect.x = hoverRect.x + Mathf.Max(hoverRectW-areaW, 0);
                hoverRect.width = areaW;
                hoverRect.height = Mathf.Min(500, hoverRect.height);
                Color bgColor = new Color(0, 0, 0, 0.5f);
                GUIEx.DrawRect( hoverRect, bgColor );

                hoverRect.height = 50;
                GUI.Label(hoverRect, _logs[hoverIndex].time +": " +_logs[hoverIndex].message);
                hoverRect.y += hoverRect.height;

                hoverRect.height = 600;
                GUI.Label(hoverRect, _logs[hoverIndex].stackTrace);
            }
        }
    }
}
