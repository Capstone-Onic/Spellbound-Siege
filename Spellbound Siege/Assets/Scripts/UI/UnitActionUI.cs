using UnityEngine;
using UnityEngine.UI;

public class UnitActionUI : MonoBehaviour
{
    public Button sellButton;
    public Button upgradeButton;

    private ClickableUnit targetUnit;

    public void Setup(ClickableUnit unit)
    {
        targetUnit = unit;

        // 연결 전에 null 체크 (중요)
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners(); // 중복 방지
            sellButton.onClick.AddListener(() =>
            {
                Debug.Log("판매 버튼 클릭됨");
                targetUnit?.SellUnit();
                Destroy(gameObject);
            });
        }
        else
        {
            Debug.LogError("[UnitActionUI] sellButton이 연결되지 않았습니다.");
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() =>
            {
                Debug.Log("강화 버튼 클릭됨");
                targetUnit?.UpgradeUnit();
                Destroy(gameObject);
            });
        }
        else
        {
            Debug.LogError("[UnitActionUI] upgradeButton이 연결되지 않았습니다.");
        }
        if (Camera.main != null)
        {
            Vector3 lookDir = Camera.main.transform.forward;
            lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
    }
}

