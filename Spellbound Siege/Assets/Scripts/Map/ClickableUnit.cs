using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableUnit : MonoBehaviour
{
    private GridTile parentTile;
    private BaseUnit baseUnit;

    [Header("유닛 액션 UI 프리팹")]
    public GameObject unitActionUIPrefab;
    private GameObject currentUI;

    // 현재 선택된 유닛 추적용
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

        // UI 위를 클릭했으면 무시
        if (EventSystem.current.IsPointerOverGameObject()) return;

        ToggleUnitUI();
    }

    private void ToggleUnitUI()
    {
        // 같은 유닛 클릭 → UI 닫기
        if (UIManager.Instance.IsUIOpen && currentlyFocusedUnit == this)
        {
            UIManager.Instance.CloseUI();
            currentlyFocusedUnit = null;
            return;
        }

        // 다른 유닛 클릭 → 기존 UI 닫고 새 UI 열기
        if (UIManager.Instance.IsUIOpen)
        {
            UIManager.Instance.CloseUI();
        }

        // UI 생성
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
            Debug.Log("[강화] 유닛 강화 완료");
            SFXManager.Instance?.PlayUpgrade();
            baseUnit.IncreaseStats();
        }
        else
        {
            Debug.Log("[강화] 골드 부족으로 강화 실패");
        }
    }
}
