using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleDetector : Part
{
    [SerializeField] private GameObject detectionArea;
    [SerializeField] private GameObject detectionIndicator;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            int range = Mathf.Clamp(value.GetValueOrDefault(), 0, 10);

            contextMenu.UpdateContextMenu("Range", range);

            detectionArea.transform.localScale = Vector2.one * range;
        }
        else
        {
            SetActive();
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        detectionArea.transform.localScale = Vector2.one * (int)parameters["Range"];
    }

    private void OnEnable()
    {
        detectionArea.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Marble marble))
        {
            AnimateDetection();
            SendTrigger();
        }
    }
    
    public override void Erase()
    {
        detectionArea.SetActive(false);

        base.Erase();
    }

    public override void SetActive()
    {
        if (isActive)
        {
            detectionArea.SetActive(false);
        }
        else 
        {
            detectionArea.SetActive(true);
        }

        base.SetActive();
    }

    public void AnimateDetection()
    {
        LeanTween.cancel(detectionIndicator);
        detectionIndicator.transform.localScale = Vector2.zero;
        LeanTween.scale(detectionIndicator, Vector2.one, 0.1f).setOnComplete( () =>
        LeanTween.scale(detectionIndicator, Vector2.one, 0.25f).setOnComplete( () =>
        LeanTween.scale(detectionIndicator, Vector2.zero, 0.2f).setEaseInBack()));
    }
}
