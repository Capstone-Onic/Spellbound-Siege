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

        BaseUnit baseUnit = unitPrefab.GetComponent<BaseUnit>();
        if (baseUnit == null)
        {
            Debug.LogWarning("BaseUnit 컴포넌트가 유닛 프리팹에 없습니다.");
            return;
        }

        if (!GoldManager.instance.SpendGold(baseUnit.goldCost))
        {
            CancelPlacement();
            return;
        }

        //유닛 생성 및 위치 설정
        currentUnit = Instantiate(unitPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        ClickableUnit clickable = currentUnit.GetComponent<ClickableUnit>();
        if (clickable != null)
        {
            clickable.SetParentTile(this);
        }

        isOccupied = true;
        isPlacing = false;
    }

    public void ClearUnit()
    {
        if (currentUnit != null)
        {
            var clickable = currentUnit.GetComponent<ClickableUnit>();
            if (clickable != null)
            {
                clickable.SetParentTile(null);
            }

            Destroy(currentUnit);
        }
        else
        {
            Debug.Log("[GridTile] currentUnit은 이미 null임");
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
        if (CameraZoomController.Instance.IsZoomed())
            return;
        if (UIManager.Instance != null && UIManager.Instance.IsUIOpen)
        {
            Debug.Log("[배치 차단] UI가 열려있음");
            return;
        }
        if (DeckBuilderManager.Instance != null && DeckBuilderManager.Instance.deckSettingPanel.activeSelf)
        {
            Debug.Log("[배치 차단] 덱 설정 패널이 열려있음");
            return;
        }
        if (FindFirstObjectByType<UnlockCardManager>()?.cardSelectPanel.activeSelf == true)
        {
            Debug.Log("[배치 차단] 보상 패널이 열려있음");
            return;
        }
        Debug.Log($"[GridTile] 유닛 설치 시도 → isOccupied={isOccupied}, currentUnit={(currentUnit == null ? "null" : currentUnit.name)}");
        // 유닛이 있으면 제거하지 않고 판매는 유닛 자체에서 처리
        if (isOccupied && currentUnit != null)
        {
            Debug.Log("[GridTile] 유닛이 이미 설치되어 있어 설치 불가");
            return;
        }

        // 빈 타일 + 선택된 유닛 있으면 배치
        if (!isOccupied &&
            UnitManager.instance != null &&
            UnitManager.instance.selectedUnit != null)
        {
            StartPlacingUnit();
        }
    }
}