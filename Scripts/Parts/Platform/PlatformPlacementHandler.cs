using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class PlatformPlacementHandler : MonoBehaviour
{   
    [Space(10)]
    [Header("Variables")]
    [SerializeField] private float snappingRange;
    [SerializeField] private float minimumPlatformLength;

    [Space(10)]
    [Header("References")]
    [SerializeField] private LineRenderer preview;
    [SerializeField] private LineRenderer previewFill;
    [SerializeField] private LineRenderer perpendicularLineRenderer;

    [SerializeField] private GameObject startPointConnectionPreview;
    [SerializeField] private GameObject endPointConnectionPreview;
    [SerializeField] private List<GameObject> overlapConnectionPreviews = new List<GameObject>();

    private List<LineRenderer> overlappingPlatforms = new List<LineRenderer>();
    private LineRenderer platformConnectedToStartPoint;
    private LineRenderer platformConnectedToEndPoint;

    private LayerMask platformLayer;
    private RaycastHit2D[] rayCastHits = new RaycastHit2D[128];
    
    private Vector2 startPoint;
    private Vector2 endPoint;

    private void Awake()
    {
        platformLayer = LayerMask.GetMask("Platform");
    }

    public void UpdatePlatformPlacement()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            InitializePlacement();
        }

        if (Input.GetMouseButton(0) && preview.enabled)
        {
            UpdateEndPointPosition();
			DrawPreview();
            CameraManager.Instance.PushCameraWithMouse();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                startPointConnectionPreview.SetActive(false);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && platformConnectedToStartPoint)
            {
                startPointConnectionPreview.SetActive(true);
            }

        }
        else 
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                UpdateStartPointConnection();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StopPlacing();
                ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ChoosingPartToPlaceState);
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                StopPlacing();
                ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.MakingSelectionState);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StopPlacing();
                ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.PlacingWireState);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
			StopPlacing();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ErasingState);
        }
        if (Input.GetMouseButtonUp(0) && preview.enabled)
        {
            if (Utilities.GetLineRendererLength(preview) >= minimumPlatformLength)
            {
                PlacePlatform();
                StopPlacing();
            }
            else 
            {
                StopPlacing();
            }
        }
    }

    private void InitializePlacement()
    {
        if (startPointConnectionPreview.activeSelf)
        {
            startPoint = startPointConnectionPreview.transform.position;
        }
        else 
        {   
            startPoint = ProgramManager.Instance.GetSnapDependantMousePosition();
        }
            
        preview.enabled = true;
        previewFill.enabled = true;
    }

    private void UpdateStartPointConnection()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            platformConnectedToStartPoint = null;
            startPointConnectionPreview.SetActive(false);
            return;
        }

        platformConnectedToStartPoint = GetClosestPlatformInRange(snappingRange)?.GetComponent<LineRenderer>();

        if (platformConnectedToStartPoint)
        {
            startPointConnectionPreview.SetActive(true);   
        }
        else 
        {
            startPointConnectionPreview.SetActive(false);
            return;
        }

        if (ProgramManager.Instance.programSettings.SnapToGrid)
        {
            startPointConnectionPreview.transform.position = GetClosestGridAlignedPointOnLineRenderer(platformConnectedToStartPoint, Utilities.GetMousePositionInWorldSpace());
            
            return;
        }
        else 
        {
            startPointConnectionPreview.transform.position = Utilities.GetClosestPointOnLineRenderer(platformConnectedToStartPoint, Utilities.GetMousePositionInWorldSpace());
            startPointConnectionPreview.SetActive(true);
        }
    }

    private void UpdateEndPointPosition()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            endPoint = ProgramManager.Instance.GetSnapDependantMousePosition();
            return;
        }

        platformConnectedToEndPoint = GetClosestPlatformInRange(snappingRange)?.GetComponent<LineRenderer>();

        // If there is a platform in range, snap to the platform
        if (platformConnectedToEndPoint)
        {
            // If the platform is straight on the grid, snap the end point to the grid as well
            if (platformConnectedToEndPoint.GetPosition(0).x == platformConnectedToEndPoint.GetPosition(1).x || platformConnectedToEndPoint.GetPosition(0).y == platformConnectedToEndPoint.GetPosition(1).y)
            {
                endPointConnectionPreview.transform.position = new Vector2(
                    (int)Utilities.GetClosestPointOnLineRenderer(platformConnectedToEndPoint, Utilities.GetMousePositionInWorldSpace()).x,
                    (int)Utilities.GetClosestPointOnLineRenderer(platformConnectedToEndPoint, Utilities.GetMousePositionInWorldSpace()).y
                );

                endPointConnectionPreview.SetActive(true);
                endPoint = endPointConnectionPreview.transform.position;
            }
            else 
            {
                endPointConnectionPreview.transform.position = Utilities.GetClosestPointOnLineRenderer(platformConnectedToEndPoint, Utilities.GetMousePositionInWorldSpace());
                endPointConnectionPreview.SetActive(true);
                endPoint = endPointConnectionPreview.transform.position;
            }

            return;
        }
        else 
        {
            endPointConnectionPreview.SetActive(false);
        }

        // If the startpoint is connected to a platform, snap to 90 degree angles from that platform
        if (platformConnectedToStartPoint)
        {
            if (platformConnectedToStartPoint.GetPosition(0).x == platformConnectedToStartPoint.GetPosition(1).x || platformConnectedToStartPoint.GetPosition(0).y == platformConnectedToStartPoint.GetPosition(1).y)
            {
                // If the connected platform is straight on the grid, we can snap the endpoint to the grid as well, while keeping the angle at 90 degrees
                if (Mathf.Abs(Utilities.GetMousePositionInWorldSpace().x - startPoint.x) < 1f)
                {
                    endPoint.x = startPoint.x;
                    endPoint.y = ProgramManager.Instance.GetSnapDependantMousePosition().y;
                    return;
                }
                if (Mathf.Abs(Utilities.GetMousePositionInWorldSpace().y - startPoint.y) < 1f)
                {
                    endPoint.y = startPoint.y;
                    endPoint.x = ProgramManager.Instance.GetSnapDependantMousePosition().x;
                    return;
                }
            }

            Vector3 platformDirection = (platformConnectedToStartPoint.GetPosition(1) - platformConnectedToStartPoint.GetPosition(0)).normalized;
            Vector3 perpendicularDirection = new Vector3(-platformDirection.y, platformDirection.x, 0f);

            perpendicularLineRenderer.SetPosition(0, (startPointConnectionPreview.transform.position - perpendicularDirection * 100f));
            perpendicularLineRenderer.SetPosition(1, (startPointConnectionPreview.transform.position + perpendicularDirection * 100f));

            if (Utilities.DistanceFromPointToLine(Utilities.GetMousePositionInWorldSpace(), perpendicularLineRenderer.GetPosition(0), perpendicularLineRenderer.GetPosition(1)) < 1f)
            {
                Vector3 projectedPoint = Utilities.GetClosestPointOnLineRenderer(perpendicularLineRenderer, Utilities.GetMousePositionInWorldSpace());
                endPoint = projectedPoint;
                return;
            }
        }
        // If there is no platform in range, and we are not snapping to 90 degree angles from a connected platform, snap to lateral directions from the connected platform
        if (Mathf.Abs(Utilities.GetMousePositionInWorldSpace().x - startPoint.x) < 1f)
        {
            endPoint.x = startPoint.x;
            endPoint.y = ProgramManager.Instance.GetSnapDependantMousePosition().y;
            return;
        }
        if (Mathf.Abs(Utilities.GetMousePositionInWorldSpace().y - startPoint.y) < 1f)
        {
            endPoint.y = startPoint.y;
            endPoint.x = ProgramManager.Instance.GetSnapDependantMousePosition().x;
            return;
        }

        // Othervise just follow the mouse
        endPoint = ProgramManager.Instance.GetSnapDependantMousePosition();
    }

    private void DrawPreview()
    {
		preview.SetPosition(0, startPoint);
		preview.SetPosition(1, endPoint);

        previewFill.SetPosition(0, startPoint);
        previewFill.SetPosition(1, endPoint);

        overlappingPlatforms.Clear();

        foreach(GameObject overlapConnectionPreview in overlapConnectionPreviews)
        {
            overlapConnectionPreview.SetActive(false);
        }

        if (Input.GetKey(KeyCode.LeftShift)) { return; }

        GetOverlappingPlatforms();

        for (int i = 0; i < overlappingPlatforms.Count; i++)
        {
            Vector2 intersectionPoint = Utilities.FindIntersection(startPoint, endPoint, overlappingPlatforms[i].GetPosition(0), overlappingPlatforms[i].GetPosition(1));
            if (!float.IsNaN(intersectionPoint.x) && !float.IsNaN(intersectionPoint.y))
            {
                overlapConnectionPreviews[i].transform.position = intersectionPoint;
                overlapConnectionPreviews[i].SetActive(true);
            }
        }
    }

    private void PlacePlatform()
    {
        Platform placedPart = (Platform)PartPoolManager.Instance.GetPart("Platform", Vector2.zero);
        placedPart.Initialize();
        placedPart.SetPositions(preview);
        placedPart.InitializeCollider();
    }

    private void StopPlacing()
    {
        preview.enabled = false;
        previewFill.enabled = false;
        
        platformConnectedToStartPoint = null;
        platformConnectedToEndPoint = null;

        startPoint = Utilities.GetMousePositionInWorldSpace();
        endPoint = startPoint;

        foreach(GameObject overlapConnectionPreview in overlapConnectionPreviews)
        {
            overlapConnectionPreview.SetActive(false);
        }

        endPointConnectionPreview.SetActive(false);
        startPointConnectionPreview.SetActive(false);
    }

    Vector2 GetClosestGridAlignedPointOnLineRenderer(LineRenderer lineRenderer, Vector2 target)
    {
        Vector2 closestPoint = Utilities.GetClosestPointOnLineRenderer(lineRenderer, target);

        float snapX = Mathf.Round(ProgramManager.Instance.GetSnapDependantMousePosition().x);
        float snapY = Mathf.Round(ProgramManager.Instance.GetSnapDependantMousePosition().y);

        float deltaX =  Mathf.Abs(lineRenderer.GetPosition(0).x - lineRenderer.GetPosition(1).x);
        float deltaY =  Mathf.Abs(lineRenderer.GetPosition(0).y - lineRenderer.GetPosition(1).y);

        if (deltaX > deltaY)
        {
            float newY = GetYAtXOnLine(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), snapX);
            newY = Mathf.Clamp(newY, Mathf.Min(lineRenderer.GetPosition(0).y, lineRenderer.GetPosition(1).y), Mathf.Max(lineRenderer.GetPosition(0).y, lineRenderer.GetPosition(1).y));
            snapX = Mathf.Clamp(snapX, Mathf.Min(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(1).x), Mathf.Max(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(1).x));
            closestPoint = new Vector2(snapX, newY);
        }
        else 
        {
            float newX = GetXAtYOnLine(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), snapY);
            newX = Mathf.Clamp(newX, Mathf.Min(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(1).x), Mathf.Max(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(1).x));
            snapY = Mathf.Clamp(snapY, Mathf.Min(lineRenderer.GetPosition(0).y, lineRenderer.GetPosition(1).y), Mathf.Max(lineRenderer.GetPosition(0).y, lineRenderer.GetPosition(1).y));
            closestPoint = new Vector2(newX, snapY);
        }

        return closestPoint;
    }

    float GetXAtYOnLine(Vector2 start, Vector2 end, float targetY)
    {
        if (Mathf.Approximately(start.x, end.x))
        {
            return start.x;
        }

        float t = (targetY - start.y) / (end.y - start.y);

        t = Mathf.Clamp01(t);

        float x = start.x + t * (end.x - start.x);

        return x;
    }

    float GetYAtXOnLine(Vector2 start, Vector2 end, float targetX)
    {
        if (Mathf.Approximately(start.y, end.y))
        {
            return start.y;
        }

        float t = (targetX - start.x) / (end.x - start.x);

        t = Mathf.Clamp01(t);

        float y = start.y + t * (end.y - start.y);

        return y;
    }

    private Platform GetClosestPlatformInRange(float range)
    {
        List <Platform> platformsInRange = new List <Platform>();

		int hitCount = Physics2D.CircleCastNonAlloc(Utilities.GetMousePositionInWorldSpace(), range, Vector2.zero, rayCastHits, Mathf.Infinity);

        for (int i = 0; i < hitCount; i++)
        {
			if (rayCastHits[i].collider.gameObject.TryGetComponent(out Platform platform))
            {
                if (!platform.connectedRotator)
                {
                    platformsInRange.Add(platform);
                }
            }
        }

        if (platformsInRange.Count == 0) { return null; }
        if (platformsInRange.Count == 1) { return platformsInRange[0]; }

        float smallestDistance = Mathf.Infinity;
        float currentDistance = 0;

        Platform closestPlatform = null;

        foreach(Platform p in platformsInRange)
        {
            currentDistance = Vector2.Distance(Utilities.GetClosestPointOnLineRenderer(p.GetComponent<LineRenderer>(), Utilities.GetMousePositionInWorldSpace()), Utilities.GetMousePositionInWorldSpace());

            if (currentDistance <= smallestDistance)
            {
                smallestDistance = currentDistance;
                closestPlatform = p;
            }
        }

        return closestPlatform;
    }

    private void GetOverlappingPlatforms()
    {
        for (int i = 0; i < preview.positionCount - 1; i++)
        {
            Vector2 startPoint = preview.GetPosition(i);
            Vector2 endPoint = preview.GetPosition(i + 1);

            RaycastHit2D[] hits = Physics2D.LinecastAll(startPoint, endPoint, platformLayer);

            foreach (RaycastHit2D hit in hits)
            {
                LineRenderer overlappingPlatform = hit.collider.GetComponent<LineRenderer>();

                if (!overlappingPlatforms.Contains(overlappingPlatform) && overlappingPlatform != null && overlappingPlatform != platformConnectedToStartPoint && overlappingPlatform != platformConnectedToEndPoint)
                {
                    overlappingPlatforms.Add(overlappingPlatform);
                }
            }
        }
    }
}
