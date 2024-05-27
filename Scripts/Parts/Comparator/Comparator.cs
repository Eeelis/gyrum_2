using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comparator : Part
{
    private int mode;
    private int operation;
    private int comparisonValue;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            if (contextMenu.valueMode == 1)
            {
                comparisonValue = value.GetValueOrDefault();
                contextMenu.UpdateContextMenu("ComparisonValue", comparisonValue);
            }
        }

        if (mode == 0)
        {
            switch(operation)
            {
                case 0:
                    if (value.GetValueOrDefault() == comparisonValue)
                    {
                        SendTrigger(value);
                    }
                    break;
                case 1:
                    if (value.GetValueOrDefault() != comparisonValue)
                    {
                        SendTrigger(value);
                    }
                    break;
                case 2:
                    if (value.GetValueOrDefault() > comparisonValue)
                    {
                        SendTrigger(value);
                    }
                    break;
                case 3:
                    if (value.GetValueOrDefault() < comparisonValue)
                    {
                        SendTrigger(value);
                    }
                    break;
            }
        }
        else 
        {
            switch(operation)
            {
                case 0:
                    if (value.GetValueOrDefault() == comparisonValue)
                    {
                        Debug.Log("here2");
                        SendTrigger();
                    }
                    break;
                case 1:
                    if (value.GetValueOrDefault() != comparisonValue)
                    {
                        SendTrigger();
                    }
                    break;
                case 2:
                    if (value.GetValueOrDefault() > comparisonValue)
                    {
                        SendTrigger();
                    }
                    break;
                case 3:
                    if (value.GetValueOrDefault() < comparisonValue)
                    {
                        SendTrigger();
                    }
                    break;
            }
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        operation = (int)parameters["Operation"];
        comparisonValue = (int)parameters["ComparisonValue"];
        mode = (int)parameters["Mode"];
    }
}
