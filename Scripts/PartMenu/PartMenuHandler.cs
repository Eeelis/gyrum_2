using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PartMenuHandler : MonoBehaviour
{
    [Space(10)]
    [Header("Variables")]
    [SerializeField] private float partMenuScale;
    
    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject partMenuContainer;
    [SerializeField] private GameObject partMenuMiddle;
    [SerializeField] private GameObject glow;
    [SerializeField] private List<PartMenuTab> partMenuTabs = new List<PartMenuTab>();
    [SerializeField] private TMP_Text partNameText;
    [SerializeField] private TMP_Text partDescriptionText;
    [SerializeField] private TMP_Text currentTabText;

    private PartMenuTab activePartMenuTab;
    private int totalMenuItems = 8;
    private int currentMenuItemIndex;
    private int previousMenuItemIndex;
    private Vector2 partMenuOpenPosition;

    private void Awake()
    {
        partMenuOpenPosition = partMenuContainer.transform.position;
        activePartMenuTab = partMenuTabs[0];
    }

    public void UpdatePartMenu()
    {
        currentMenuItemIndex = GetCurrentMenuItemIndex();

        if (currentMenuItemIndex != previousMenuItemIndex)
        {
            UpdateInfoText();

            activePartMenuTab.items[previousMenuItemIndex].UnHighlight();
            activePartMenuTab.items[currentMenuItemIndex].Highlight();
            
            previousMenuItemIndex = currentMenuItemIndex;
        }
    }

    private void UpdateInfoText()
    {
        partDescriptionText.text = GetActiveMenuItem().partMenuDescription;
        partNameText.text = GetActiveMenuItem().partName;
    }

    private int GetCurrentMenuItemIndex()
    {
        Vector2 normalizedMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
        float currentAngle = ((Mathf.Atan2(normalizedMousePosition.y, normalizedMousePosition.x) * Mathf.Rad2Deg) + 360) % 360;

        return Mathf.FloorToInt(currentAngle / (360f / totalMenuItems));
    }

    public Part GetActiveMenuItem()
    {
        if (activePartMenuTab)
        {
            return activePartMenuTab.items[currentMenuItemIndex].associatedPart;
        }

        return null;
    }

    public void OpenPartMenu()
    {
        LeanTween.cancel(currentTabText.gameObject);
        LeanTween.cancel(partMenuContainer);

        partMenuContainer.transform.localScale = Vector2.zero;
        currentTabText.transform.localScale = Vector2.zero;

        currentTabText.gameObject.SetActive(true);
        partMenuMiddle.SetActive(true);
        partMenuContainer.SetActive(true);

        LeanTween.scale(partMenuContainer, Vector2.one * partMenuScale, 0.3f).setEaseOutBack().setIgnoreTimeScale(true);
        LeanTween.scale(currentTabText.gameObject, Vector2.one * partMenuScale, 0.3f).setEaseOutBack().setIgnoreTimeScale(true);
    }

    public void ClosePartMenu()
    {
        currentTabText.gameObject.SetActive(false);
        partMenuMiddle.SetActive(false);
        LeanTween.cancel(partMenuContainer);
        LeanTween.scale(partMenuContainer, Vector2.zero, 0.25f).setOnComplete( () => partMenuContainer.SetActive(true) ).setEaseOutCubic().setIgnoreTimeScale(true);
    }

    public void SwitchTab(bool nextTab)
    {
        LeanTween.cancel(partMenuContainer);
        partMenuContainer.transform.position = partMenuOpenPosition;

        activePartMenuTab.items[GetCurrentMenuItemIndex()].UnHighlight();
        activePartMenuTab.gameObject.SetActive(false);

        if (nextTab)
        {
            activePartMenuTab = partMenuTabs[(partMenuTabs.IndexOf(activePartMenuTab) + 1 ) % partMenuTabs.Count];
        }
        else
        {
            activePartMenuTab = partMenuTabs[(partMenuTabs.IndexOf(activePartMenuTab) - 1 + partMenuTabs.Count) % partMenuTabs.Count];
        }

        LeanTween.cancel(currentTabText.gameObject);
        currentTabText.transform.localScale = Vector2.one * partMenuScale;
        LeanTween.scale(currentTabText.gameObject, (Vector2.one * 1.6f) * partMenuScale, 0.4f).setEasePunch().setIgnoreTimeScale(true);

        currentTabText.text = $"{partMenuTabs.IndexOf(activePartMenuTab) + 1} / {partMenuTabs.Count}";

        activePartMenuTab.gameObject.SetActive(true);
        activePartMenuTab.items[GetCurrentMenuItemIndex()].Highlight();
        UpdateInfoText();
    }
}