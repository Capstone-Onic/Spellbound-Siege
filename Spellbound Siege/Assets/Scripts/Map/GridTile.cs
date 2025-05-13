using UnityEngine;

public class GridTile : MonoBehaviour
{
    public GameObject unitShadowPrefab;
    private GameObject currentUnit;
    private GameObject currentShadow;
    private bool isOccupied = false;
    private bool isPlacing = false;

    public bool isPathTile = false; // 경로 여부

    private void OnMouseDown()
    {
        // 1. 게임 시작 전 (처음 유닛 선택 가능)
        if (!StartGameManager.gameStarted)
        {
            if (isPathTile) return;

            TryPlaceOrRemoveUnit();
            return;
        }

        // 2. 게임 시작 후 + 라운드 종료 상태 (배치 시간)
        if (StartGameManager.isPlacementPhase)
        {
            if (isPathTile) return;

            TryPlaceOrRemoveUnit();
            return;
        }

        // 3. 그 외에는 무시 (라운드 진행 중)
    }

    private void Update()
    {
        if (isPlacing && currentShadow != null)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GridTile currentTile = hit.collider.GetComponent<GridTile>();
                    if (currentTile != this)
                    {
                        CancelPlacement();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                PlaceUnit();
            }
        }
    }
    private void StartPlacingUnit()
    {
        if (!isOccupied)
        {
            Vector3 shadowPosition = transform.position + new Vector3(0, 0.2f, 0);
            currentShadow = Instantiate(unitShadowPrefab, shadowPosition, Quaternion.identity);
            isPlacing = true;
        }
    }

    private void PlaceUnit()
    {
        if (currentShadow != null)
            Destroy(currentShadow);

        GameObject unitPrefab = UnitManager.instance.selectedUnit;

        //BaseUnit에서 설치 비용 가져오기
        BaseUnit baseUnit = unitPrefab.GetComponent<BaseUnit>();
        if (baseUnit == null)
        {
            Debug.LogWarning("BaseUnit 컴포넌트가 유닛 프리팹에 없습니다.");
            return;
        }

        //골드 차감 시도
        if (!GoldManager.instance.SpendGold(baseUnit.goldCost))
        {
            CancelPlacement(); // 골드 부족 → 배치 취소
            return;
        }

        //유닛 설치
        currentUnit = Instantiate(unitPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        isOccupied = true;
        isPlacing = false;
    }

    public void ClearUnit()
    {
        if (currentUnit != null)
        {
            Destroy(currentUnit);
        }

        currentUnit = null;
        isOccupied = false;
    }

    private void CancelPlacement()
    {
        if (currentShadow != null)
        {
            Destroy(currentShadow);
        }

        isPlacing = false;
    }

    // 경로 타일 설정
    public void SetAsPathTile()
    {
        isPathTile = true;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = new Color(0.4f, 0.4f, 0.4f); // 회색
        }
    }
    private void TryPlaceOrRemoveUnit()
    {
        if (isOccupied && currentUnit != null)
        {
            ClearUnit();
            return;
        }

        if (!isOccupied &&
            UnitManager.instance != null &&
            UnitManager.instance.selectedUnit != null)
        {
            StartPlacingUnit();
        }
    }
}