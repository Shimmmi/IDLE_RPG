using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestRewardDisplay : MonoBehaviour
{
    public Image rewardImage;          // UI Image рядом с игроком
    public float displayTime = 2f;     // Сколько секунд показывать
    public AnimationCurve fadeCurve;   // Кривая появления/затухания

    private Coroutine displayRoutine;

    public void ShowReward(Sprite itemSprite)
    {
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        rewardImage.sprite = itemSprite;
        displayRoutine = StartCoroutine(DisplayCoroutine());
    }

    private IEnumerator DisplayCoroutine()
    {
        float time = 0f;
        var cg = rewardImage.GetComponent<CanvasGroup>();
        cg.alpha = 0f;

        // Fade in
        while (time < displayTime)
        {
            time += Time.deltaTime;
            float t = time / displayTime;
            cg.alpha = fadeCurve.Evaluate(t);
            yield return null;
        }

        // Fade out
        float fadeOutTime = 1f;
        time = 0f;
        while (time < fadeOutTime)
        {
            time += Time.deltaTime;
            float t = 1f - time / fadeOutTime;
            cg.alpha = fadeCurve.Evaluate(t);
            yield return null;
        }

        cg.alpha = 0f;
        displayRoutine = null;
    }
}
