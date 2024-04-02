using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject sendersContainer;
    [SerializeField] private GameObject receiversContainer;
    [SerializeField] private GameObject connectionMatrix;
    [SerializeField] private ConnectionMatrixElement connectionMatrixElementPrefab;
    [SerializeField] private GameObject hideConnectionMatrixButtonIcon;

    protected Part associatedPart;
    protected ContextMenuData currentContextMenuData = new ContextMenuData();

    private Dictionary<Part, ConnectionMatrixElement> connectionMatrixElements = new Dictionary<Part, ConnectionMatrixElement>();
    private bool connectionMatrixOpen = true;
    private float connectionMatrixOpenPosition = 0;
    private float connectionMatrixClosedPosition = -245;

    public abstract void UpdateContextMenu(string parameter, object value);

    public abstract void Initialize(Part part);

    public virtual void SendContextMenuDataToAssociatedPart()
    {
        if (!associatedPart) { return; }
        
        associatedPart.ReceiveContextMenuData(currentContextMenuData);
    }

    public virtual void AddReceiverToConnectionMatrix(Part receiver)
    {
        if (connectionMatrixElements.ContainsKey(receiver))
        { 
            return;
        }

        ConnectionMatrixElement newConnectionMatrixElement = Instantiate(connectionMatrixElementPrefab, receiversContainer.transform);
        newConnectionMatrixElement.transform.SetAsFirstSibling();
        newConnectionMatrixElement.Initialize(associatedPart, receiver, receiver);
        connectionMatrixElements.Add(receiver, newConnectionMatrixElement);

        UpdateConnectionMatrixPanelPositions();
    }

    public virtual void AddSenderToConnectionMatrix(Part sender)
    {
        if (connectionMatrixElements.ContainsKey(sender))
        { 
            return;
        }
        
        ConnectionMatrixElement newConnectionMatrixElement = Instantiate(connectionMatrixElementPrefab, sendersContainer.transform);
        newConnectionMatrixElement.transform.SetAsFirstSibling();
        newConnectionMatrixElement.Initialize(sender, associatedPart, sender);
        connectionMatrixElements.Add(sender, newConnectionMatrixElement);

        UpdateConnectionMatrixPanelPositions();
    }

    public virtual void RemoveConnectionMatrixElement(Part part)
    {
        if (connectionMatrixElements.ContainsKey(part))
        {
            Destroy(connectionMatrixElements[part].gameObject);
            connectionMatrixElements.Remove(part);
        }
    }

    private void OnEnable()
    {
        UpdateConnectionMatrixPanelPositions();

        if (ProgramManager.Instance.programSettings.ShowConnectionMatrixOnPartSelect)
        {
            ShowConnectionMatrix(false);
        }
        else
        {
            HideConnectionMatrix(false);
        }
    }

    public virtual void UpdateConnectionMatrixPanelPositions()
    {
        if (sendersContainer)
        {
            sendersContainer.transform.position = new Vector2(sendersContainer.transform.position.x, -100f);
        }

        if (receiversContainer)
        {
            receiversContainer.transform.position = new Vector2(receiversContainer.transform.position.x, -100f);
        }
    }

    public void ShowOrHideConnectionMatrix()
    {
        if (connectionMatrixOpen)
        {
            HideConnectionMatrix(true);
        }
        else 
        {
            ShowConnectionMatrix(true);
        }
    }

    public void ShowConnectionMatrix(bool animate)
    {
        connectionMatrixOpen = true;

        ProgramManager.Instance.programSettings.ShowConnectionMatrixOnPartSelect = true;

        if (animate)
        {
            LeanTween.cancel(connectionMatrix);
            LeanTween.moveLocalX(connectionMatrix, connectionMatrixOpenPosition, 0.15f).setEaseOutBack().setIgnoreTimeScale(true);
        }
        else 
        {
            LeanTween.cancel(connectionMatrix);
            connectionMatrix.transform.localPosition = new Vector2(connectionMatrixOpenPosition, connectionMatrix.transform.localPosition.y);
        }

        hideConnectionMatrixButtonIcon.transform.localRotation = new Quaternion(0f, 0f, 180f, 0f);
    }

    public void HideConnectionMatrix(bool animate)
    {
        connectionMatrixOpen = false;

        ProgramManager.Instance.programSettings.ShowConnectionMatrixOnPartSelect = false;

        if (animate)
        {
            LeanTween.cancel(connectionMatrix);
            LeanTween.moveLocalX(connectionMatrix, connectionMatrixClosedPosition, 0.15f).setEaseInBack().setIgnoreTimeScale(true);
        }
        else 
        {
            LeanTween.cancel(connectionMatrix);
            connectionMatrix.transform.localPosition = new Vector2(connectionMatrixClosedPosition, connectionMatrix.transform.localPosition.y);
        }

        hideConnectionMatrixButtonIcon.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    public ContextMenuData GetContextMenuData()
    {
        return currentContextMenuData;
    }
}
