using UnityEngine;

public class ClickOutsideToClose : MonoBehaviour
{
    public GameObject panelToCheck;
    public GameObject extraPanelToIgnore; // ← SmithPanel

    private RectTransform rectTransform;
    private RectTransform extraRect;

    void Start()
    {
        if (panelToCheck != null)
            rectTransform = panelToCheck.GetComponent<RectTransform>();
        if (extraPanelToIgnore != null)
            extraRect = extraPanelToIgnore.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (panelToCheck.activeSelf && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;

            bool outsideMain = !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, null);
            bool outsideExtra = extraRect == null || !RectTransformUtility.RectangleContainsScreenPoint(extraRect, mousePosition, null);

            if (outsideMain && outsideExtra)
            {
                panelToCheck.SetActive(false);

                if (extraPanelToIgnore != null)
                    extraPanelToIgnore.SetActive(false);
            }
        }
    }
}
