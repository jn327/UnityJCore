using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameOptionsSO gameOptions;
    public StateMachine<GameManager> stateMachine;

    private DebugManager _debugManager;
    public DebugManager debugManager
    {
        get { return _debugManager; }
    }

    void Awake()
    {
        _debugManager = GetComponent<DebugManager>();
    }
}
