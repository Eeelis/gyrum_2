using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comparator : Part
{
    private int operation = 0;
    private int storedValue;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            switch(operation)
            {
                case 0:
                    if (value.GetValueOrDefault() == storedValue)
                    {
                        SendTrigger(value);
                    }
                break;
                case 1:
                    if (value.GetValueOrDefault() != storedValue)
                    {
                        SendTrigger(value);
                    }
                break;
                case 2:
                    if (value.GetValueOrDefault() > storedValue)
                    {
                        SendTrigger(value);
                    }
                break;
                case 3:
                    if (value.GetValueOrDefault() < storedValue)
                    {
                        SendTrigger(value);
                    }
                break;
            }
        }
    }

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        operation = contextMenuData.GetParameter<int>("Operation");
        storedValue = contextMenuData.GetParameter<int>("StoredValue");
    }
}
