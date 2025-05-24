using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionUI : MonoBehaviour
{
    public Button sellButton;
    public Button upgradeButton;
    public Button zoomOutButton;

    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI sellValueText;

    private ClickableUnit targetUnit;
    public void Setup(ClickableUnit unit)
    {
        targetUnit = unit;
        string levelText = targetUnit.upgradeLevel >= 2 ? "Lv.MAX" : $"Lv.{targetUnit.upgradeLevel + 1}";

        if (!CameraZoomController.Instance.IsZoomed())
        {
            CameraZoomController.Instance.ZoomToUnit(unit.transform);
        }
        if (unitNameText != null)
            unitNameText.text = $"{targetUnit.unitName} {levelText}";

        if (upgradeCostText != null)
            upgradeCostText.text = $"{targetUnit.GetUpgradeCost()}";

        if (sellValueText != null)
            sellValueText.text = $"{targetUnit.GetSellValue()}";

        // 연결 전에 null 체크 (중요)
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners(); // 중복 방지
            sellButton.onClick.AddListener(() =>
            {
                Debug.Log("판매 버튼 클릭됨");
                targetUnit?.SellUnit();
                UIManager.Instance.CloseUI(resetCamera: true);
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
                if (unitNameText != null)
                    levelText = targetUnit.upgradeLevel >= 2 ? "Lv.MAX" : $"Lv.{targetUnit.upgradeLevel + 1}";
                    unitNameText.text = $"{targetUnit.unitName} {levelText}";
                    upgradeCostText.text = targetUnit.upgradeLevel >= 2 ? "-" : $"{targetUnit.GetUpgradeCost()}";
                    sellValueText.text = $"{targetUnit.GetSellValue()}";
            });
        }
        else
        {
            Debug.LogError("[UnitActionUI] upgradeButton이 연결되지 않았습니다.");
        }
        
        zoomOutButton.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseUI(resetCamera: true);
        });

    }
}

