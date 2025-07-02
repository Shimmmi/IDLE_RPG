using UnityEngine;
using UnityEngine.UI;

public class DebugUpgradePointButton : MonoBehaviour
{
    public UpgradeSystem upgradeSystem;
    public Button debugButton;

    void Start()
    {
        debugButton.onClick.AddListener(() =>
        {
            if (upgradeSystem != null)
            {
                upgradeSystem.GainUpgradePoint();
                Debug.Log("Тест: Очко улучшения добавлено!");
            }
        });
    }
}
