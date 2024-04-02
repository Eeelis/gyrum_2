using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartMenuItem : MonoBehaviour
{

    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] public Part associatedPart;

    private RectTransform iconContainer;
    private GameObject highlight;

    private void Awake()
    {
        iconContainer = transform.GetChild(0).GetComponent<RectTransform>();
        highlight = iconContainer.transform.GetChild(0).gameObject;
        highlight.SetActive(false);
    }

    public void Highlight()
    {
        highlight.SetActive(true);
        LeanTween.scale(GetComponent<RectTransform>(), new Vector2(1.05f, 1.05f), 0.1f).setEaseOutBack().setIgnoreTimeScale(true);
    }

    public void UnHighlight()
    {
        LeanTween.cancel(gameObject);
        highlight.SetActive(false);
        LeanTween.scale(GetComponent<RectTransform>(), new Vector2(1f, 1f), 0.1f).setEase(LeanTweenType.easeInOutBounce).setIgnoreTimeScale(true);
    }
}
