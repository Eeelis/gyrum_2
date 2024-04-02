using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : Part
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer fill;
    [SerializeField] private EdgeCollider2D edgeCollider;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color permeableColor;

    private List<Vector2> gridAlignedPoints = new List<Vector2>();

    private int bounciness;

    public void SetPositions(LineRenderer lr)
    {
        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);
        lineRenderer.positionCount = lr.positionCount;
        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true;
        
        fill.positionCount = lr.positionCount;
        fill.SetPositions(positions);
    }

    public void InitializeCollider()
    {
        List<Vector2> edges = new List<Vector2>();

        for (int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.SetPoints(edges);
    }

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        bounciness = contextMenuData.GetParameter<int>("Bounciness");
        edgeCollider.enabled = false;
        edgeCollider.sharedMaterial.bounciness = bounciness / 5f;
        edgeCollider.enabled = true;
    }

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        { 
            bounciness = Mathf.Clamp(value.GetValueOrDefault(), 0, 5);
            contextMenu.UpdateContextMenu("Bounciness", bounciness);

            edgeCollider.enabled = false;
            edgeCollider.sharedMaterial.bounciness = bounciness / 5f;
            edgeCollider.enabled = true;

            return;
        }


        if (gameObject.layer == LayerMask.NameToLayer("Permeable"))
        {
            gameObject.layer = LayerMask.NameToLayer("Platform");
            lineRenderer.startColor = lineRenderer.endColor = defaultColor;
        }
        else  
        {
            gameObject.layer = LayerMask.NameToLayer("Permeable");
            lineRenderer.startColor = lineRenderer.endColor = permeableColor;
        }
    }

    public override void Erase()
    {
        gameObject.layer = LayerMask.NameToLayer("Platform");
        lineRenderer.startColor = lineRenderer.endColor = defaultColor;
        base.Erase();
    }

    
}
