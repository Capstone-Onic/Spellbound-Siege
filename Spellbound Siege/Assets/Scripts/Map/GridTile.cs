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
        if (StartGameManager.gameStarted) return;
        if (isPathTile) return; // 경로 타일엔 설치 금지

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
        {
            Destroy(currentShadow);
        }
        currentUnit = Instantiate(UnitManager.instance.selectedUnit, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        isOccupied = true;
        isPlacing = false;

        ClickableUnit cu = currentUnit.GetComponent<ClickableUnit>();
        if (cu != null)
        {
            cu.SetParentTile(this);
        }
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
}