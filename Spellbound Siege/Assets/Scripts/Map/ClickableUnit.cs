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

        ToggleUnitUI();
    }

    private void ToggleUnitUI()
    {
        // ���� ���� Ŭ�� �� UI �ݱ�
        if (UIManager.Instance.IsUIOpen && currentlyFocusedUnit == this)
        {
            UIManager.Instance.CloseUI();
            currentlyFocusedUnit = null;
            return;
        }

        // �ٸ� ���� Ŭ�� �� ���� UI �ݰ� �� UI ����
        if (UIManager.Instance.IsUIOpen)
        {
            UIManager.Instance.CloseUI();
        }

        // UI ����
        Renderer rend = GetComponentInChildren<Renderer>();
        float unitHeight = rend != null ? rend.bounds.size.y : 2f;
        Vector3 spawnPos = transform.position + Vector3.up * (unitHeight + 0.2f);

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
            int refund = baseUnit.goldCost / 2;
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
        int upgradeCost = 10;

        if (GoldManager.instance.SpendGold(upgradeCost))
        {
            Debug.Log("[��ȭ] ���� ��ȭ �Ϸ�");
            SFXManager.Instance?.PlayUpgrade();
            baseUnit.IncreaseStats();
        }
        else
        {
            Debug.Log("[��ȭ] ��� �������� ��ȭ ����");
        }
    }
}
