using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Part
{
    private int count = 0;
    private int threshold;

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        threshold = contextMenuData.GetParameter<int>("Threshold");
        count = contextMenuData.GetParameter<int>("Count");
    }

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            int newThreshold = Mathf.Clamp(value.GetValueOrDefault(), 1, 999);
            threshold = newThreshold;
            contextMenu.UpdateContextMenu("Threshold", newThreshold);
        }
        else 
        {
            count++;

            if (count >= threshold)
            {
                SendTrigger();
                count = 0;
            }

            contextMenu.UpdateContextMenu("Count", count);
        }
    }
}
