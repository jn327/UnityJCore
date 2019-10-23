using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugOptions", menuName = "ScriptableObjects/DebugOptions", order = 2)]
public class DebugOptionsSO : ScriptableObject
{
    public bool showFPS = true;
    public bool showLog = true;
}
