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

    private int bounciness;

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
        else
        {
            SetActive();
        }
    }
    
    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        bounciness = (int)parameters["Bounciness"];
        
        edgeCollider.enabled = false;
        edgeCollider.sharedMaterial.bounciness = bounciness / 5f;
        edgeCollider.enabled = true;
    }

    public void SetPositions(LineRenderer lr)
    {
        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);
        lineRenderer.positionCount = lr.positionCount;
        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true;
        
        fill.positionCount = lr.positionCount;
        fill.SetPositions(positions);

               
        selectionIndicator.GetComponent<LineRenderer>().positionCount = lr.positionCount;
        selectionIndicator.GetComponent<LineRenderer>().SetPositions(positions);
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

    public override void SetActive()
    {
        if (isActive)
        {
            gameObject.layer = LayerMask.NameToLayer("Permeable");
            lineRenderer.startColor = lineRenderer.endColor = permeableColor;
        }
        else  
        {
            gameObject.layer = LayerMask.NameToLayer("Platform");
            lineRenderer.startColor = lineRenderer.endColor = defaultColor;
        }

        base.SetActive();
    }

    public override void Erase()
    {
        cachedCollider.enabled = false;

        Vector2 targetPos = Vector2.zero;

        if (connectedRotator)
        {
            targetPos = connectedRotator.transform.position;
        }
        else 
        {
            targetPos = Utilities.GetLineRendererMiddlePoint(lineRenderer);
        }

        LeanTween.value(gameObject, (Vector2)lineRenderer.GetPosition(1), targetPos, 0.075f).setOnUpdate( (Vector2 value) =>
        {
            fill.SetPosition(1, value);
            lineRenderer.SetPosition(1, value);
        });
        LeanTween.value(gameObject, (Vector2)lineRenderer.GetPosition(0), targetPos, 0.075f).setOnUpdate( (Vector2 value) =>
        {
            fill.SetPosition(0, value);
            lineRenderer.SetPosition(0, value);
        }).setOnComplete( () =>
        {
            gameObject.layer = LayerMask.NameToLayer("Platform");
            lineRenderer.startColor = lineRenderer.endColor = defaultColor;
            base.Erase();
        });
    }
}
