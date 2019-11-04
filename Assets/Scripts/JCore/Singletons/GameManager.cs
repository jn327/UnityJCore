using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameOptionsSO gameOptions;
    public DebugOptionsSO debugOptions;

    public StateMachine<GameManager> stateMachine;

    private DebugLog _debugLog;
    public DebugLog debugLog
    {
        get { return _debugLog; }
    }

    void Awake()
    {
        _debugLog = gameObject.AddComponent<DebugLog>();
    }
}
