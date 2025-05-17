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

        // ���� ���� null üũ (�߿�)
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners(); // �ߺ� ����
            sellButton.onClick.AddListener(() =>
            {
                Debug.Log("�Ǹ� ��ư Ŭ����");
                targetUnit?.SellUnit();
                Destroy(gameObject);
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
                Destroy(gameObject);
            });
        }
        else
        {
            Debug.LogError("[UnitActionUI] upgradeButton�� ������� �ʾҽ��ϴ�.");
        }
        if (Camera.main != null)
        {
            Vector3 lookDir = Camera.main.transform.forward;
            lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
    }
}

