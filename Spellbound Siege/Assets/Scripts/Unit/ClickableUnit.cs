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
    public string unitName = "기사";
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

        // UI 위를 클릭했으면 무시
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (CameraZoomController.Instance.IsZoomed() && UIManager.Instance.IsUIOpen && currentlyFocusedUnit != this)
            return;

        ToggleUnitUI();
    }

    private void ToggleUnitUI()
    {
        if (CameraZoomController.Instance.IsZoomed() && currentlyFocusedUnit != this)
            return;
        
        // UI 생성
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
        // 먼저 강화 가능 여부 확인
        if (baseUnit.upgradeLevel >= 2)
        {
            Debug.Log("[강화] 이미 최대 레벨입니다.");
            return;
        }

        int upgradeCost = baseUnit.GetUpgradeCost();

        // 그 다음 골드 충분한지 확인하고 차감
        if (GoldManager.instance.SpendGold(upgradeCost))
        {
            Debug.Log("[강화] 유닛 강화 완료");
            SFXManager.Instance?.PlayUpgrade();
            baseUnit.IncreaseStats();
            upgradeLevel = baseUnit.upgradeLevel;
        }
        else
        {
            Debug.Log("[강화] 골드 부족으로 강화 실패");
        }
    }
}
