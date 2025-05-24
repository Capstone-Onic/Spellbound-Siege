using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableUnit : MonoBehaviour
{
    private GridTile parentTile;
    private BaseUnit baseUnit;

    [Header("���� �׼� UI ������")]
    public GameObject unitActionUIPrefab;
    private GameObject currentUI;

    // ���� ���õ� ���� ������
    public static ClickableUnit currentlyFocusedUnit;
    public string unitName = "���";
    public int upgradeLevel = 0;
    
    public int GetUpgradeCost()
    {
        return baseUnit.GetUpgradeCost();
    }

    public int GetSellValue()
    {
        return baseUnit.GetSellValue();
    }

    public void SetParentTile(GridTile tile)
    {
        parentTile = tile;
    }

    private void Start()
    {
        baseUnit = GetComponent<BaseUnit>();
    }

    private void OnMouseDown()
    {
        if (!StartGameManager.isPlacementPhase) return;

        // UI ���� Ŭ�������� ����
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (CameraZoomController.Instance.IsZoomed() && UIManager.Instance.IsUIOpen && currentlyFocusedUnit != this)
            return;

        ToggleUnitUI();
    }

    private void ToggleUnitUI()
    {
        if (CameraZoomController.Instance.IsZoomed() && currentlyFocusedUnit != this)
            return;
        
        // UI ����
        Renderer rend = GetComponentInChildren<Renderer>();
        float unitHeight = rend != null ? rend.bounds.size.y : 2f;
        Vector3 spawnPos = transform.position + Vector3.up * baseUnit.uiOffsetY;

        currentUI = Instantiate(unitActionUIPrefab, spawnPos, Quaternion.identity);
        currentUI.transform.SetParent(null);
        UIManager.Instance.OpenUI(currentUI);

        UnitActionUI ui = currentUI.GetComponent<UnitActionUI>();
        if (ui != null)
        {
            ui.Setup(this);
        }

        currentlyFocusedUnit = this;
    }

    public void SellUnit()
    {
        if (baseUnit != null)
        {
            int refund = baseUnit.GetSellValue();
            GoldManager.instance.AddGold(refund);
            baseUnit.CleanupUpgradeEffects();
        }

        SFXManager.Instance?.PlaySell();

        if (parentTile != null)
        {
            parentTile.ClearUnit();
            parentTile = null;
        }

        Destroy(gameObject);
    }

    public void UpgradeUnit()
    {
        // ���� ��ȭ ���� ���� Ȯ��
        if (baseUnit.upgradeLevel >= 2)
        {
            Debug.Log("[��ȭ] �̹� �ִ� �����Դϴ�.");
            return;
        }

        int upgradeCost = baseUnit.GetUpgradeCost();

        // �� ���� ��� ������� Ȯ���ϰ� ����
        if (GoldManager.instance.SpendGold(upgradeCost))
        {
            Debug.Log("[��ȭ] ���� ��ȭ �Ϸ�");
            SFXManager.Instance?.PlayUpgrade();
            baseUnit.IncreaseStats();
            upgradeLevel = baseUnit.upgradeLevel;
        }
        else
        {
            Debug.Log("[��ȭ] ��� �������� ��ȭ ����");
        }
    }
}
