using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SelectionHandler : MonoBehaviour
{ 
    [Space(10)]
    [Header("Variables")]
    [SerializeField] private float lineWidth;

    private List<Part> selectedParts = new List<Part>();
    private BoxCollider2D boxSelectionCollider;
    private LineRenderer lineRenderer;
    private Vector3[] boxSelectionCorners = new Vector3[4];
    private bool drawingSelection;
    private Part lastDeselectedPart;

    private void Awake()
    {
        boxSelectionCollider = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;

        drawingSelection = false;
        lineRenderer.enabled = false;
        boxSelectionCollider.enabled = false; 
    }

    public void SelectPart(Part part)
    {   
        if (part == null) { return; }

        if (!selectedParts.Contains(part))
        {
            if (GetLastSelectedPart() && part.contextMenu)
            {
                GetLastSelectedPart().HideContextMenu(false);
            }

            part.Select();
            selectedParts.Add(part);

            if (selectedParts.Count > 1)
            {
                part.ShowContextMenu(false);
            }
            else 
            {
                if (part.contextMenu)
                {
                    lastDeselectedPart?.HideContextMenu(false);
                    part.ShowContextMenu(true);
                }
            }
        }
    }

    public bool PartsSelected()
    {
        return selectedParts.Count > 0;
    }

    public void DeselectPart(Part part)
    {
        if (part == null) { return; }

        if (selectedParts.Contains(part))
        {
            lastDeselectedPart = part;
            part.Deselect();
            selectedParts.Remove(part);

            if (GetLastSelectedPart())
            {
                part.HideContextMenu(false);
                GetLastSelectedPart().ShowContextMenu(false);
            }
            else 
            {
                part.HideContextMenu(true);
            }
        }
    }

    public void DeselectAllParts()
    {
        foreach(Part part in selectedParts.ToArray())
        {
            if (part)
            { 
                DeselectPart(part);
            }
        }

        selectedParts.Clear();
    }

    public Part GetLastSelectedPart()
    {
        if (selectedParts.Count > 0)
        {
            return selectedParts[selectedParts.Count - 1];
        }

        return null;
    }

    
    public void InitializeBoxSelection(bool deselectAllParts)
    {
        if (selectedParts.Contains(Utilities.GetPartUnderPointer()))
        {
            DeselectPart(Utilities.GetPartUnderPointer());
        }
        else 
        {
            if (deselectAllParts)
            {
                DeselectAllParts();
            }

            SelectPart(Utilities.GetPartUnderPointer());
        }

        // The first corner of the selection box is the position of the mouse as its clicked
        boxSelectionCorners[0] = Utilities.GetMousePositionInWorldSpace();

        lineRenderer.enabled = true;
        boxSelectionCollider.enabled = true;
    }

    public void DrawBoxSelection()
    {
        // Find the remaining corners of the selection box
        boxSelectionCorners[2] = Utilities.GetMousePositionInWorldSpace();
        boxSelectionCorners[1] = new Vector2 (boxSelectionCorners[2].x, boxSelectionCorners[0].y);
        boxSelectionCorners[3] = new Vector2 (boxSelectionCorners[0].x, boxSelectionCorners[2].y);

        drawingSelection = (Vector2.Distance(boxSelectionCorners[0], boxSelectionCorners[2]) > 0.5f);

        // Draw the selection box and scale its collider accordingly
        lineRenderer.SetPositions(boxSelectionCorners);
        boxSelectionCollider.size = new Vector2(Vector2.Distance(boxSelectionCorners[0], boxSelectionCorners[1]), Vector2.Distance(boxSelectionCorners[1], boxSelectionCorners[2]));
        boxSelectionCollider.offset = (boxSelectionCorners[0] + boxSelectionCorners[2]) / 2;

        CameraManager.Instance.PushCameraWithMouse();
    }

    public void EndBoxSelection()
    {
        drawingSelection = false;
        lineRenderer.enabled = false;
        boxSelectionCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D partCollider)
    {
        if (partCollider.gameObject.TryGetComponent(out Part part))
        {
            SelectPart(part);
        }
    }

    private void OnTriggerExit2D(Collider2D partCollider)
    {
        if (drawingSelection && partCollider.gameObject.TryGetComponent(out Part part))
        {
            DeselectPart(part);
        }
    }
}
