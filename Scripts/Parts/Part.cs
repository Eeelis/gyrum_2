using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Part : MonoBehaviour
{
    [SerializeField] public string partName;
    [TextAreaAttribute]
    [SerializeField] public string contextMenuDescription;
    [TextAreaAttribute]
    [SerializeField] public string partMenuDescription;

    [Space(10)]
    [SerializeField] public Settings settings;
    [Space(10)]
    [SerializeField] public Prefabs prefabs;

    [HideInInspector] public List<Part> senders = new List<Part>();
    [HideInInspector] public List<Part> receivers = new List<Part>();
    [HideInInspector] public ContextMenu contextMenu;

    public static Action<Part> OnPartSelected;
    public static Action<Part> OnPartDeselected;

    [System.Serializable]
    public struct Settings
    {
        public bool canSendTriggers;
        public bool canReceiveTriggers;
        public bool canSendValues;
        public bool canReceiveValues;
        public bool canBeConnectedToRotators;
        public bool canBeConnectedToRails;
        public bool animatePlacement;
        public bool animateErasement;
        public bool animateWireConnetion;
    }

    private struct RigidbodyInfo
    {
        public CollisionDetectionMode2D collisionDetectionMode;
        public RigidbodyType2D bodyType;
        public bool simulated;
    }

    private RigidbodyInfo rigidbodyInfo;

    private float contextMenuOpenPosition;
    private float contextMenuClosedPosition;

    [HideInInspector] public Rigidbody2D cachedRigidbody;
    [HideInInspector] public Collider2D cachedCollider;
    [HideInInspector] public SpriteRenderer cachedSpriteRenderer;
    [HideInInspector] public Part connectedRotator;

    private Quaternion originalRotation;

    [System.Serializable]
    public struct Prefabs
    {
        public ContextMenu contextMenuPrefab;
        public GameObject partPreviewPrefab;
    }

    public virtual void Initialize()
    { 
        if (prefabs.contextMenuPrefab && !contextMenu)
        {
            contextMenu = Instantiate(prefabs.contextMenuPrefab, GameObject.Find("ContextMenuContainer").transform);
            contextMenu.Initialize(this);
            contextMenuOpenPosition = contextMenu.transform.position.x;
            contextMenuClosedPosition = contextMenuOpenPosition - 400f;
        }
        else if (contextMenu)
        {
            contextMenu.Initialize(this);
        }

        cachedRigidbody = GetComponent<Rigidbody2D>();
        cachedCollider = GetComponent<Collider2D>();
        cachedSpriteRenderer = GetComponent<SpriteRenderer>();

        originalRotation = transform.rotation;
    }

    public void DisableRigidbody()
    {
        if (!cachedRigidbody) { return; }

        rigidbodyInfo.bodyType = cachedRigidbody.bodyType;
        rigidbodyInfo.simulated = cachedRigidbody.simulated;
        rigidbodyInfo.collisionDetectionMode = cachedRigidbody.collisionDetectionMode;

        Destroy(cachedRigidbody);
    }

    public void EnableRigidbody()
    {
        if (cachedRigidbody == null)
        {
            cachedRigidbody = gameObject.AddComponent<Rigidbody2D>();
            cachedRigidbody.bodyType = rigidbodyInfo.bodyType;
            cachedRigidbody.simulated = rigidbodyInfo.simulated;
            cachedRigidbody.collisionDetectionMode = rigidbodyInfo.collisionDetectionMode;
        }
    }

    public virtual void Erase()
    {
        foreach(Part sender in senders.ToArray())
        {
            ConnectionManager.Instance.RemoveConnection(sender, this);
        }

        foreach(Part receiver in receivers.ToArray())
        {
            ConnectionManager.Instance.RemoveConnection(this, receiver);
        }

        if (connectedRotator)
        {
            connectedRotator.Erase();
            return;
        }

        if (settings.animateErasement)
        {
            LeanTween.scale(gameObject, Vector2.zero, 0.1f).setEaseInBack().setOnComplete( () => PartPoolManager.Instance.ReturnPart(this)).setIgnoreTimeScale(true);
        }
        else 
        {
            PartPoolManager.Instance.ReturnPart(this);
        }
    }

    public void ResetVelocity()
    {
        if (!cachedRigidbody) { return; }

        cachedRigidbody.velocity = Vector2.zero;
    }

    public void ResetRotation()
    {
        transform.rotation = originalRotation;
    }

    private void OnDisable()
    {
        HideContextMenu(false);
    }

    public virtual void ShowContextMenu(bool animate)
    {
        if (!contextMenu) { return; }

        if (animate)
        {
            LeanTween.cancel(contextMenu.gameObject);
            contextMenu.gameObject.transform.position = new Vector2(contextMenuClosedPosition, contextMenu.transform.position.y);
            contextMenu.gameObject.SetActive(true);
            LeanTween.moveX(contextMenu.gameObject, contextMenuOpenPosition, 0.2f).setEaseOutExpo();
        }
        else 
        {
            contextMenu.gameObject.transform.position = new Vector2(contextMenuOpenPosition, contextMenu.transform.position.y);
            contextMenu.gameObject.SetActive(true);
        }
    }

    public virtual void HideContextMenu(bool animate)
    {
        if (!contextMenu) { return; }

        if (animate)
        {
            LeanTween.cancel(contextMenu.gameObject);
            LeanTween.moveX(contextMenu.gameObject, contextMenuClosedPosition, 0.2f).setEaseInBack().setOnComplete(() => contextMenu.gameObject.SetActive(false));
        }
        else 
        {
            contextMenu.gameObject.transform.position = new Vector2(contextMenuClosedPosition, contextMenu.transform.position.y);
            contextMenu.gameObject.SetActive(false);
        }
    }

    public virtual void RemoveSender(Part part)
    {
        senders.Remove(part);
    }

    public virtual void RemoveReceiver(Part part)
    {
        receivers.Remove(part);
    }

    public virtual void AddSender(Part part)
    {
        senders.Add(part);
    }

    public virtual void AddReceiver(Part part)
    {
        receivers.Add(part);
    }

    public virtual void SendTrigger(int? value = null)
    {
        foreach(Part receiver in receivers.ToArray())
        {
            receiver.ReceiveTrigger(value);
        }
    }

    public virtual void ReceiveTrigger(int? value)
    {

    }

    public virtual void Select()
    {
        OnPartSelected?.Invoke(this);
    }

    public virtual void Deselect()
    {
        OnPartDeselected?.Invoke(this);
    }

    public virtual void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {

    }

    public virtual void Reset()
    {
        connectedRotator = null;

        LeanTween.cancel(gameObject);
        EnableRigidbody();
        ResetRotation();
        ResetVelocity();
    }
}
