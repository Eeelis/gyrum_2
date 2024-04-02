using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VTTConverter : Part
{
    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            SendTrigger();
        }
    }
}
