using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Part
{
    public void Clicked()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.one;
        LeanTween.scale(gameObject, transform.localScale * 1.2f, 0.25f).setEasePunch();
        SendTrigger();
    }
}
