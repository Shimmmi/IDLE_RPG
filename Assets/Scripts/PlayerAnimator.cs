using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimator : MonoBehaviour
{
    private Image playerImage;

    void Start()
    {
        playerImage = GetComponent<Image>();
    }

    public void AnimateOnClick()
    {
        // ���������� ��������� ����
        Color newColor = new Color(Random.value, Random.value, Random.value);
        playerImage.color = newColor;
    }
}
