using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonContextMenu : ContextMenu
{
    public override void Initialize(Part associatedPart)
    {
        this.associatedPart = associatedPart;
    }
    
    public override void UpdateContextMenu(string parameterName, object value)
    {
        return;
    }
}
