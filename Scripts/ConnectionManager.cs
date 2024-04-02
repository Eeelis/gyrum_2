using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    [SerializeField] private GameObject wireViewOverlay;

    private Dictionary<(Part, Part), Wire> connections = new Dictionary<(Part, Part), Wire>();

    private void Awake()
    {
        Instance = this;
    }

    public void ShowWires()
    {
        wireViewOverlay.SetActive(true);

        foreach(Wire wire in connections.Values)
        {
            wire.gameObject.SetActive(true);
        }
    }

    public void HideWires()
    {
        wireViewOverlay.SetActive(false);

        foreach(Wire wire in connections.Values)
        {
            wire.gameObject.SetActive(false);
        }
    }

    public void RemoveConnection(Part sender, Part receiver)
    {
        sender.contextMenu?.RemoveConnectionMatrixElement(receiver);
        receiver.contextMenu?.RemoveConnectionMatrixElement(sender);

        sender.RemoveReceiver(receiver);
        receiver.RemoveSender(sender);
        Destroy(connections[(sender, receiver)].gameObject);
        connections.Remove((sender, receiver));
    }

    public void ConnectParts(Part sender, Part receiver, Wire wire)
    {
        var compositeKey = (sender, receiver);

        if (!connections.ContainsKey(compositeKey))
        {
            connections.Add(compositeKey, wire);
        }

        sender.contextMenu?.AddReceiverToConnectionMatrix(receiver);
        receiver.contextMenu?.AddSenderToConnectionMatrix(sender);

        sender.AddReceiver(receiver);
        receiver.AddSender(sender);
    }
}
