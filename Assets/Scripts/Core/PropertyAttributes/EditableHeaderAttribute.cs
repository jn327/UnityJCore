// This is not an editor script. The property attribute class should be placed in a regular script file.
using UnityEngine;

public class EditableHeaderAttribute : PropertyAttribute
{
    public string[] propertyNames;
    public bool showButtons;

    public EditableHeaderAttribute( string[] propertyNames, bool showButtons = true )
    {
        this.propertyNames = propertyNames;
        this.showButtons = showButtons;
    }
}