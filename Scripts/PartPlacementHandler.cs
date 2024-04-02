using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PartPlacementHandler : MonoBehaviour
{
    [Space(10)]
    [Header("References")]
    [SerializeField] private SelectionHandler selectionHandler;
    [SerializeField] private PlatformPlacementHandler platformPlacementHandler;
    [SerializeField] private RotatorPlacementHandler rotatorPlacementHandler;
    [SerializeField] private Transform partPreviewContainer;

    private Collider2D previewCollider;
    private Collider2D[] overlaps = new Collider2D[8];
    private ContactFilter2D contactFilter = new ContactFilter2D();

    private Dictionary<string, GameObject> partPreviews = new Dictionary<string, GameObject>();
    private GameObject activePartPreview;
    private Part partToPlace;

    private void Start()
    {
        InstantiateAndStorePartPreviews();
    }

    private void InstantiateAndStorePartPreviews()
    {
        foreach(Part part in PartPoolManager.Instance.GetEachPartType())
        {
            if (part.prefabs.partPreviewPrefab)
            {
                GameObject partPreview = Instantiate(part.prefabs.partPreviewPrefab, partPreviewContainer);
                partPreview.SetActive(false);
                partPreviews.Add(part.partName, partPreview);
            }
        }
    }

    public void Initialize(Part partToPlace)
    {
        this.partToPlace = partToPlace;
        
        if (partToPlace is Rotator)
        {
            rotatorPlacementHandler.InitializeRotatorPlacement();
        }
        else if (partToPlace)
        {
            if (partPreviews.ContainsKey(partToPlace.partName))
            {
                activePartPreview = partPreviews[partToPlace.partName];
                previewCollider = activePartPreview.GetComponent<Collider2D>();
                activePartPreview.transform.position = ProgramManager.Instance.GetSnapDependantMousePosition();
                activePartPreview.SetActive(true);
            }
        }
    }

    public bool IsPreviewInOccupiedSpace()
    {
        if (previewCollider)
        {
            return previewCollider.OverlapCollider(contactFilter, overlaps) > 0;
        }

        return false;
    }

    public void GetInput()
    {
        if (partToPlace is Platform)
        {
            platformPlacementHandler.UpdatePlatformPlacement();
            return;
        }
        else if (partToPlace is Rotator)
        {
            rotatorPlacementHandler.UpdateRotatorPlacement();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ChoosingPartToPlaceState);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.MakingSelectionState);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.PlacingWireState);
        }
        if (Input.GetMouseButtonDown(1))
        {
            selectionHandler.DeselectAllParts();
            ProgramManager.Instance.RequestStateChange(ProgramManager.ProgramState.ErasingState);
        }

        if (activePartPreview.activeInHierarchy)
        {
            activePartPreview.transform.position = ProgramManager.Instance.GetSnapDependantMousePosition();

            if (EventSystem.current.IsPointerOverGameObject())
            {
                ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Default);
                return;
            }
            if (IsPreviewInOccupiedSpace())
            {
                ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Prohibited);
                return;
            }
            else 
            {
                ProgramManager.Instance.SetCursorType(ProgramManager.CursorType.Default);
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlacePart();
            }
        }
    }

    public void PlacePart()
    {
        Part placedPart = PartPoolManager.Instance.GetPart(partToPlace.partName, ProgramManager.Instance.GetSnapDependantMousePosition());

        if (partToPlace.settings.animatePlacement)
        {
            placedPart.transform.localScale = Vector2.zero;
            LeanTween.scale(placedPart.gameObject, Vector2.one, 0.15f).setEaseOutBack();
        }

        placedPart.Initialize();
    }

    public void CancelPlacement()
    {
        activePartPreview?.SetActive(false);
    }


}
