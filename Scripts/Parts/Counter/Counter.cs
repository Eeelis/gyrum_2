using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Part
{
    private int count;
    private int threshold;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            int newThreshold = Mathf.Clamp(value.GetValueOrDefault(), 1, 999);
            threshold = newThreshold;
            contextMenu.UpdateContextMenu("Threshold", newThreshold);
        }
        else if (isActive)
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

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        threshold = (int)parameters["Threshold"];
        count = (int)parameters["Count"];
    }
}
