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
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 1.2f;
        currentUI = Instantiate(unitActionUIPrefab, spawnPos, Quaternion.identity);

        // 1. 부모 제거: 유닛 회전 영향 차단
        currentUI.transform.SetParent(null);

        // 2. 카메라 정면 바라보도록 회전 설정
        if (Camera.main != null)
        {
            // 정확히 카메라를 바라보는 방향으로 앞면 강제
            Vector3 cameraForward = Camera.main.transform.forward;
            currentUI.transform.forward = cameraForward;
        }

        // 3. 버튼 기능 연결
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
