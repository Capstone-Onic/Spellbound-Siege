using UnityEngine;

public class GridTile : MonoBehaviour
{
    public GameObject unitShadowPrefab;
    private GameObject currentUnit;
    private GameObject currentShadow;
    private bool isOccupied = false;
    private bool isPlacing = false;

    private void OnMouseDown()
    {
        if (!isOccupied && UnitManager.instance != null && UnitManager.instance.selectedUnit != null)
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
                    if (currentTile != null && currentTile != this)
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

        // 유닛이 삭제된 경우 상태 초기화
        if (isOccupied && currentUnit == null)
        {
            isOccupied = false;
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
    }

    private void CancelPlacement()
    {
        if (currentShadow != null)
        {
            Destroy(currentShadow);
        }

        isPlacing = false;
    }
}