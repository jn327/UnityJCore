using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameOptionsSO gameOptions;

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
