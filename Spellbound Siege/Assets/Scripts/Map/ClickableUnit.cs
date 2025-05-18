using UnityEngine;

public class ClickableUnit : MonoBehaviour
{
    private GridTile parentTile;
    private BaseUnit baseUnit;

    [Header("유닛 액션 UI 프리팹")]
    public GameObject unitActionUIPrefab;
    private GameObject currentUI;

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

        ToggleUnitUI();
    }

    private void ToggleUnitUI()
    {
        if (UIManager.Instance.IsUIOpen)
        {
            // 같은 유닛 다시 누르면 토글 닫기
            if (UIManager.Instance.currentUnitUI == currentUI)
            {
                UIManager.Instance.CloseUI();
                return;
            }

            // 다른 UI가 열려 있으면 닫기
            UIManager.Instance.CloseUI();
        }

        Renderer rend = GetComponentInChildren<Renderer>();
        float unitHeight = rend != null ? rend.bounds.size.y : 2f;
        Vector3 spawnPos = transform.position + Vector3.up * (unitHeight + 0.2f);

        currentUI = Instantiate(unitActionUIPrefab, spawnPos, Quaternion.identity);
        currentUI.transform.SetParent(null);
        UIManager.Instance.OpenUI(currentUI); // UI 등록

        UnitActionUI ui = currentUI.GetComponent<UnitActionUI>();
        if (ui != null)
        {
            ui.Setup(this);
        }
    }


    public void SellUnit()
    {
        if (baseUnit != null)
        {
            int refund = baseUnit.goldCost / 2;
            GoldManager.instance.AddGold(refund);
        }

        if (parentTile != null)
        {
            parentTile.ClearUnit();
            parentTile = null;
        }

        Destroy(gameObject);
    }

    public void UpgradeUnit()
    {
        int upgradeCost = 10; // 임의 설정

        if (GoldManager.instance.SpendGold(upgradeCost))
        {
            Debug.Log("[강화] 유닛 강화 완료");

            baseUnit.IncreaseStats(); // 외형/스탯 변화용 함수 (아직 내부 구현 안함)
        }
        else
        {
            Debug.Log("[강화] 골드 부족으로 강화 실패");
        }
    }
}
