using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : Part
{
    public override void ReceiveTrigger(int? value)
    {
        if (receivers.Count > 0 && isActive)
        {
            int randomIndex = Random.Range(0, receivers.Count);
            Part randomReceiver = receivers[randomIndex];

            randomReceiver.ReceiveTrigger(value);
        }
    }
}
