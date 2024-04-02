using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Part
{
    private int rate;
    private bool ticking;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            value = Mathf.Clamp(value.GetValueOrDefault(), 0, 999);
            this.rate = value.GetValueOrDefault();
            contextMenu.UpdateContextMenu("Rate", value.GetValueOrDefault());

            if (rate > 0 && ticking)
            {
                CancelInvoke("Tick");
                InvokeRepeating("Tick", 60f / rate, 60f / rate);
            }

            return;
        }

        if (ticking)
        {
            CancelInvoke("Tick");
            ticking = false;
        }
        else
        {
            ticking = true;

            if (rate > 0)
            {
                InvokeRepeating("Tick", 60f / rate, 60f / rate);
            }
        }
    }

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        rate = contextMenuData.GetParameter<int>("Rate");
        
        CancelInvoke("Tick");

        if (rate > 0)
        {
            ticking = true;
            InvokeRepeating("Tick", 60f / rate, 60f / rate);
        }
    }

    private void Tick()
    {
        SendTrigger();
    }
}
