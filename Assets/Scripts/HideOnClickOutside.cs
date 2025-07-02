using UnityEngine;

public class HideOnClickOutside : MonoBehaviour
{
    public GameObject panelToHide;
    public GameObject extraPanelToIgnore; // ← сюда перетащим smithPanel

    private RectTransform panelRect;
    private RectTransform extraRect;

    void Start()
    {
        if (panelToHide != null)
            panelRect = panelToHide.GetComponent<RectTransform>();
        if (extraPanelToIgnore != null)
            extraRect = extraPanelToIgnore.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (panelToHide.activeSelf && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            bool outsideMain = !RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePosition, null);
            bool outsideExtra = extraRect == null || !RectTransformUtility.RectangleContainsScreenPoint(extraRect, mousePosition, null);

            if (outsideMain && outsideExtra)
            {
                panelToHide.SetActive(false);
            }
        }
    }
}
