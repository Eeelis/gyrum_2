using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager Instance;

    [Space(10)]
    [Header("References")]
    [SerializeField] private GridBuilder gridBuilder;
    [SerializeField] private PartMenuHandler partMenuHandler;
    [SerializeField] private PartPlacementHandler partPlacementHandler;
    [SerializeField] private SelectionHandler selectionHandler;
    [SerializeField] private WirePlacementHandler wirePlacementHandler;

    [Space(10)]
    [Header("Cursors")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D erasingCursor;
    [SerializeField] private Texture2D prohibitedCursor;

    [HideInInspector]
    public enum CursorType
    {
        Default,
        Erasing,
        Prohibited,
    }

    [HideInInspector]
    public struct ProgramSettings
    {
        public bool SnapToGrid;
        public bool Muted;
        public bool ShowConnectionMatrixOnPartSelect;
    }

    [HideInInspector]
    public enum ProgramState
    {
        ChoosingPartToPlaceState,
        PlacingPartState,
        ErasingState,
        MakingSelectionState,
        NoneState,
        PlacingWireState
    }

    [HideInInspector] public ProgramSettings programSettings;

    private IState currentState;
    private ProgramState previousState;

    private void Awake()
    {
        Instance = this;

        gridBuilder.OnGridGenerated += OnGridGenerated;

        currentState = new NoneState();

        SetCursorType(CursorType.Default);

        programSettings.ShowConnectionMatrixOnPartSelect = true;
        programSettings.SnapToGrid = true;
    }

    private void OnGridGenerated(Cell[,] grid)
    {
        // Center the camera on the grid
        CameraManager.Instance.SetCameraPosition(new Vector2((grid.GetLength(0) / 2) -0.5f, (grid.GetLength(1) / 2) - 0.5f), false);
    }
    
    // Create new state and inject any required dependencies
    public void RequestStateChange(ProgramState state)
    {
        switch((state))
        {
            case (ProgramState.NoneState):
                ChangeState(new NoneState());
                break;
            case (ProgramState.ChoosingPartToPlaceState):
                ChangeState(new ChoosingPartToPlaceState(partMenuHandler));
                break;
            case (ProgramState.PlacingPartState):
                if (partMenuHandler.GetActiveMenuItem())
                {
                    ChangeState(new PlacingPartState(partMenuHandler.GetActiveMenuItem(), partPlacementHandler));
                }
                else 
                {
                    ChangeState(new NoneState());
                }
                break;
            case (ProgramState.MakingSelectionState):
                ChangeState(new MakingSelectionState(selectionHandler));
                break;
            case (ProgramState.ErasingState):
                ChangeState(new ErasingState(selectionHandler));
                break;
            case (ProgramState.PlacingWireState):
                ChangeState(new PlacingWireState(wirePlacementHandler));
                break;
        }
    }

    public void ChangeStateToPreviousState()
    {
        RequestStateChange(previousState);
    }

    public void ChangeState(IState newState)
    {
        // Only certain states should be stored as previousState, as the other states require specific input to enter
        if ((currentState is PlacingWireState) || (currentState is PlacingPartState) || (currentState is NoneState))
        {
            previousState = ConvertStateToEnum(currentState);
        }
        
        currentState?.Exit();

        currentState = newState;

        currentState?.Enter();
    }

    public Vector2 GetSnapDependantMousePosition()
    {
        if (programSettings.SnapToGrid)
        {
            return gridBuilder.GetCellAtPosition(Utilities.GetMousePositionInWorldSpace()).transform.position;
        }

        return Utilities.GetMousePositionInWorldSpace(); 
    }

    public void SetCursorType(CursorType type)
    {
        switch(type)
        {
            case CursorType.Default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Erasing:
                Cursor.SetCursor(erasingCursor, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Prohibited:
                Cursor.SetCursor(prohibitedCursor, Vector2.zero, CursorMode.ForceSoftware);
                break;
        }
    }

    // Converts an Istate to a ProgramManager.ProgramState. Useful as some functions require the enum representation, while others require the actual class
    private ProgramState ConvertStateToEnum(IState state)
    {
        switch(state)
        {
            case ErasingState _ when state.GetType() == typeof(ErasingState):
                return ProgramState.ErasingState;
            case PlacingPartState _ when state.GetType() == typeof(PlacingPartState):
                return ProgramState.PlacingPartState;
            case MakingSelectionState _ when state.GetType() == typeof(MakingSelectionState):
                return ProgramState.MakingSelectionState;
            case PlacingWireState _ when state.GetType() == typeof(PlacingWireState):
                return ProgramState.PlacingWireState;
            default:
                return ProgramState.NoneState;
        }
    }

    private void Update()
    {
        currentState?.UpdateState();

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            programSettings.SnapToGrid = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            programSettings.SnapToGrid = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1f;
            }
            else 
            {
                Time.timeScale = 0f;
            }
        }

    }
}    