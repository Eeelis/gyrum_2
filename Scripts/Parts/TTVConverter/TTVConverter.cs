using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTVConverter : Part
{
    private int storedValue;

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        this.storedValue = contextMenuData.GetParameter<int>("StoredValue");
    }

    public override void ReceiveTrigger(int? value)
    {
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
}
