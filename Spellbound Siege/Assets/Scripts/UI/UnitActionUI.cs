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

        // ���� ���� null üũ (�߿�)
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners(); // �ߺ� ����
            sellButton.onClick.AddListener(() =>
            {
                Debug.Log("�Ǹ� ��ư Ŭ����");
                targetUnit?.SellUnit();
                UIManager.Instance.CloseUI(resetCamera: true);
            });
        }
        else
        {
            Debug.LogError("[UnitActionUI] sellButton�� ������� �ʾҽ��ϴ�.");
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() =>
            {
                Debug.Log("��ȭ ��ư Ŭ����");
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
            Debug.LogError("[UnitActionUI] upgradeButton�� ������� �ʾҽ��ϴ�.");
        }
        
        zoomOutButton.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseUI(resetCamera: true);
        });

    }
}

