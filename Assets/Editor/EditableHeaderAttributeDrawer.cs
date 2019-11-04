// The property drawer class should be placed in an editor script, inside a folder called Editor.

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EditableHeaderAttribute))]
[CanEditMultipleObjects]
public class EditableHeaderAttributeDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    private int _elementHeight = 15;
    private int _spacing = 5;
    private int _buttonWidth = 20;

    private int _arrowXOffset = 16;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditableHeaderAttribute namesAttribute = attribute as EditableHeaderAttribute;
        SerializedObject serializedObj = property.serializedObject;
        //serializedObj.UpdateIfRequiredOrScript();
        
        //find out if we can show the buttons
        float buttonWidth = 0;
        bool isArrayOrList = false;
        System.Type targetObjectType = fieldInfo.GetValue(serializedObj.targetObject).GetType();
        if ( targetObjectType.IsArray || targetObjectType.IsGenericType )
        {
            isArrayOrList = true;
        }
        bool showButtons = namesAttribute.showButtons && isArrayOrList;
        if (showButtons)
        {
            buttonWidth += _spacing + _buttonWidth + _spacing + _buttonWidth;
        }

        //check if it has any of the properties and display them
        int propertiesFound     = 0;
        int nPropertiesToCheck  = namesAttribute.propertyNames.Length;
        
        float availableWidth    = position.width - buttonWidth - _spacing;
        Rect propertyRect       = new Rect( position.x + _spacing, position.y, availableWidth, _elementHeight );

        float totalFixedWidth   = 0;
        int nFixedElements      = 0;
        for (int i = 0; i < nPropertiesToCheck; i++)
        {
            SerializedProperty namedProperty = property.FindPropertyRelative(namesAttribute.propertyNames[i]);
            if (namedProperty != null)
            {
                if (namedProperty.propertyType == SerializedPropertyType.Boolean)
                {
                    totalFixedWidth = buttonWidth;
                    nFixedElements ++;
                }
            }
        }
        float availableWidthStep = (availableWidth - totalFixedWidth) / (nPropertiesToCheck - nFixedElements);
        //availableWidthStep -= ((nPropertiesToCheck-(nFixedElements+1)) * _spacing);

        for (int i = 0; i < nPropertiesToCheck; i++)
        {
            SerializedProperty namedProperty = property.FindPropertyRelative(namesAttribute.propertyNames[i]);
            if (namedProperty != null)
            {
                propertiesFound ++;

                if (namedProperty.propertyType == SerializedPropertyType.Boolean)
                {
                    propertyRect.width = buttonWidth;
                }
                else if (namedProperty.propertyType == SerializedPropertyType.Float)
                {
                    int offsetX = 0;
                    if (i == 0)
                    {
                        offsetX = _arrowXOffset;
                    }
                    propertyRect.width = availableWidthStep - offsetX;
                    propertyRect.x += offsetX;
                }
                else
                {
                    propertyRect.width = availableWidthStep;
                }

                EditorGUI.PropertyField(propertyRect, namedProperty, new GUIContent(""), false);
            }
            else
            {
                int offsetX = 0;
                if (i == 0)
                {
                    offsetX = _arrowXOffset;
                }
                propertyRect.width = availableWidthStep - offsetX;
                propertyRect.x += offsetX;

                EditorGUI.HelpBox ( propertyRect, property.type+"."+namesAttribute.propertyNames[i], MessageType.Error );
            }
            
            availableWidth -= propertyRect.width;
            propertyRect.x += propertyRect.width;
            //if (i < nPropertiesToCheck - 1)
            //{
            //    propertyRect.x += _spacing;
            //}
        }
        
        // buttons
        if (showButtons)
        {
            // duplicate button 
            Rect buttonRect = new Rect(position.x + (position.width-(_buttonWidth + _buttonWidth + _spacing)), position.y, _buttonWidth, _elementHeight);
            if (EditorGUI.DropdownButton(buttonRect, new GUIContent("+"), FocusType.Keyboard, new GUIStyle(GUI.skin.button) ))
            {
                property.DuplicateCommand();
            }
            
            // delete button
            buttonRect = new Rect(position.x + (position.width-_buttonWidth), position.y, _buttonWidth, _elementHeight);
            if (EditorGUI.DropdownButton(buttonRect, new GUIContent("-"), FocusType.Keyboard, new GUIStyle(GUI.skin.button) ))
            {
                property.DeleteCommand();
            }
        }
        
        // show the rest of the content in the normal fashion
        EditorGUI.PropertyField(position, property, new GUIContent(""), true); 

        //serializedObj.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property);
    }
}