﻿using UnityEngine;

public static class GUIEx 
{
	public static readonly Texture2D backgroundTexture = Texture2D.whiteTexture;
    public static readonly GUIStyle textureStyle = new GUIStyle {normal = new GUIStyleState { background = backgroundTexture } };

	public static void DrawRect(Rect position, Color color, GUIContent content = null)
    {
        Color backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUI.Box(position, content != null ? content : GUIContent.none, textureStyle);
        GUI.backgroundColor = backgroundColor;
    }
}
