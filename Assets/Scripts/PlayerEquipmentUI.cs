using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PlayerEquipmentUI : MonoBehaviour
{
    public static PlayerEquipmentUI instance;
    private Coroutine rotateCoroutine;
    private float targetRotationZ = 0f;
    public Image equippedIcon; // ← перетащи сюда иконку поверх PlayerImage

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (equippedIcon == null)
        {
            Debug.LogError("EquippedIcon не назначен в PlayerEquipmentUI!");
        }

        HideEquippedIcon();
    }

    public void ShowEquippedIcon(Sprite icon)
    {
        if (equippedIcon == null) return;
        
        equippedIcon.sprite = icon;
        equippedIcon.enabled = true;
        equippedIcon.rectTransform.rotation = Quaternion.identity; // сброс поворота
        targetRotationZ = 0f;

    }

    public void HideEquippedIcon()
    {
        if (equippedIcon == null) return;
        
        equippedIcon.enabled = false;
        equippedIcon.rectTransform.rotation = Quaternion.identity; // сброс на всякий
    }

    public void RotateEquippedIcon()
    {
        if (equippedIcon == null || !equippedIcon.enabled)
            return;

        targetRotationZ -= 30f;

        if (rotateCoroutine == null)
            rotateCoroutine = StartCoroutine(SmoothRotate());
    }


    private IEnumerator SmoothRotate()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        Quaternion startRot = equippedIcon.rectTransform.rotation;
        Quaternion endRot = Quaternion.Euler(0, 0, targetRotationZ);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            equippedIcon.rectTransform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        equippedIcon.rectTransform.rotation = endRot;
        rotateCoroutine = null;
    }
}
