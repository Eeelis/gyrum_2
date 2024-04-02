using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleDetector : Part
{
    [SerializeField] private GameObject detectionArea;
    [SerializeField] private GameObject detectionIndicator;

    private void OnEnable()
    {
        detectionArea.SetActive(true);
    }

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        detectionArea.transform.localScale = Vector2.one * contextMenuData.GetParameter<int>("Range");
    }

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            int range = value.GetValueOrDefault();

            contextMenu.UpdateContextMenu("Range", range);

            detectionArea.transform.localScale = Vector2.one * range;
        }
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

    public void AnimateDetection()
    {
        LeanTween.cancel(detectionIndicator);
        detectionIndicator.transform.localScale = Vector2.zero;
        LeanTween.scale(detectionIndicator, Vector2.one, 0.35f).setEaseOutBack().setOnComplete( () =>
        LeanTween.scale(detectionIndicator, Vector2.zero, 0.3f).setEaseInBack());
    }
}
