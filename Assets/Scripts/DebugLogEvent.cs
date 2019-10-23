using UnityEngine;

public class DebugLogEvent 
{
	private float _time;
	private string _message;
	private UnityEngine.Color _color;
	private string _stackTrace;
	
	//TODO:
	//private int _fps;
	//private UnityEngine.Texture2D _screenshot;

	// =======================
	//		Constructors
	// =======================
	public DebugLogEvent( string message, UnityEngine.Color color )
	{
		init( message, color);
	}

	public DebugLogEvent( string message )
	{
		init( message, UnityEngine.Color.gray);
	}

	private void init( string message, UnityEngine.Color color )
	{
		//_fps		= GameManager.debugManager.currentFPS;
		_time		= UnityEngine.Time.time;
		_stackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace ();
		_message	= message;
		_color		= color;

		sendToUnityEngine();
	}

	public void sendToUnityEngine()
	{
		UnityEngine.Debug.Log("<color=#"+ColorUtility.ToHtmlStringRGB(_color)+">" +_time +": " +_message+ "</color>");
	}

	// =======================
	//		Getters
	// =======================
	public UnityEngine.Color color
    {
        get { return _color; }
    }

	public string message
    {
        get { return _message; }
    }

	public float time
    {
        get { return _time; }
    }

	public float stackTrace
    {
        get { return _stackTrace; }
    }
}
