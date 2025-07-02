using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingIconUI : MonoBehaviour
{
    public static FloatingIconUI instance;
    public Image iconImage;

    public Vector3 finalScale = Vector3.one; // конечный размер (обычно 1,1,1)
    public Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f); // начальный размер
    public float scaleDuration = 0.3f; // время анимации появления

    public float fadeDuration = 0.2f; // время затухания (прозрачность)

    private RectTransform rectTransform;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        rectTransform = iconImage.GetComponent<RectTransform>();
        Hide();
    }

    public void ShowIcon(Sprite icon, float duration = 1.5f)
    {
        iconImage.sprite = icon;
        iconImage.enabled = true;
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0f); // полностью прозрачная

        rectTransform.localScale = startScale;

        StartCoroutine(PlayPopUpAnimation(duration));
    }

    private IEnumerator PlayPopUpAnimation(float duration)
    {
        // 1. Анимация появления (затухание + увеличение)
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / scaleDuration);
            iconImage.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
            rectTransform.localScale = Vector3.Lerp(startScale, finalScale, t);
            yield return null;
        }

        iconImage.color = Color.white;
        rectTransform.localScale = finalScale;

        // 2. Ждём заданное время
        yield return new WaitForSeconds(duration);

        // 3. Скрываем с плавным исчезновением
        StartCoroutine(HideWithFade());
    }

    private IEnumerator HideWithFade()
    {
        float elapsed = 0f;
        Color startColor = iconImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            iconImage.color = Color.Lerp(startColor, new Color(1, 1, 1, 0), t);
            yield return null;
        }

        Hide();
    }

    public void Hide()
    {
        iconImage.enabled = false;
        iconImage.color = Color.white;
        rectTransform.localScale = startScale;
    }
}