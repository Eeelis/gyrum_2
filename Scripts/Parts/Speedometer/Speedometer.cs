using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : Part
{
    [SerializeField] private float updateRate;
    [SerializeField] private GameObject detectionArea;
    [SerializeField] private LineRenderer detectionIndicator;

    private Marble marbleToTrack;
    private float timer;

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

            Debug.Log(range);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Marble marble))
        {
            marbleToTrack = marble;
            timer = updateRate;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Marble marble))
        {
            if (marble == marbleToTrack)
            {
                marbleToTrack = null;
            }
        }
    }

    private void Update()
    {
        if (!marbleToTrack)
        { 
            detectionIndicator.enabled = false;
            return;
        }
        
        detectionIndicator.enabled = true;
        detectionIndicator.SetPosition(0, transform.position);
        detectionIndicator.SetPosition(1, marbleToTrack.transform.position);

        SendTrigger((int)marbleToTrack.cachedRigidbody.velocity.magnitude);
        
        timer += Time.deltaTime;

        if (timer >= updateRate)
        {
            timer = 0;
        } 
    }

    
    public override void Erase()
    {
        detectionIndicator.enabled = false;
        detectionArea.SetActive(false);

        base.Erase();
    }
}
