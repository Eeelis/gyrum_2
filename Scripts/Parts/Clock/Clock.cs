using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Part
{
    private int rate;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            rate = Mathf.Clamp(value.GetValueOrDefault(), 0, 999);
            contextMenu.UpdateContextMenu("Rate", rate);

            if (rate > 0 && isActive)
            {
                CancelInvoke("Tick");
                InvokeRepeating("Tick", 60f / rate, 60f / rate);
            }
        }
        else 
        {
            SetActive();
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        rate = (int)parameters["Rate"];

        CancelInvoke("Tick");

        if (rate > 0)
        {
            InvokeRepeating("Tick", 60f / rate, 60f / rate);
        }
    }

    public override void SetActive()
    {
        if (isActive)
        {
            CancelInvoke("Tick"); 
        }
        else if (rate > 0)
        {
            InvokeRepeating("Tick", 60f / rate, 60f / rate);
        }

        base.SetActive();
    }

    private void Tick()
    {
        SendTrigger();
    }

    public override object GetState()
    {
        return new State()
        {
            rate = this.rate,
            isActive = this.isActive
        };
    }

    public override void LoadState(object s)
    {
        var state = (State)s;
        rate = state.rate;
        isActive = state.isActive;
        
        contextMenu?.UpdateContextMenu("Rate", rate);
    }

    [System.Serializable]
    private struct State
    {
        public int rate;
        public bool isActive;
    }
}
