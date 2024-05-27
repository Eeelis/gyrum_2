using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTVConverter : Part
{
    private int storedValue;

    public override void ReceiveTrigger(int? value)
    {
        if (!isActive) { return; }
        
        if (value.HasValue)
        {
            this.storedValue = value.GetValueOrDefault();
            contextMenu.UpdateContextMenu("StoredValue", value.GetValueOrDefault());
        }
        else 
        {
            SendTrigger(storedValue);
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        storedValue = (int)parameters["StoredValue"];
    }
}
