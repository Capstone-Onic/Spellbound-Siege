using UnityEngine;

public class GridTile : MonoBehaviour
{
    public GameObject unitShadowPrefab;
    private GameObject currentUnit;
    private GameObject currentShadow;
    private bool isOccupied = false;
    private bool isPlacing = false;

    public bool isPathTile = false; // ��� ����

    private void OnMouseDown()
    {
        // 1. ���� ���� �� (ó�� ���� ���� ����)
        if (!StartGameManager.gameStarted)
        {
            if (isPathTile) return;

            TryPlaceOrRemoveUnit();
            return;
        }

        // 2. ���� ���� �� + ���� ���� ���� (��ġ �ð�)
        if (StartGameManager.isPlacementPhase)
        {
            if (isPathTile) return;

            TryPlaceOrRemoveUnit();
            return;
        }

        // 3. �� �ܿ��� ���� (���� ���� ��)
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

        //BaseUnit���� ��ġ ��� ��������
        BaseUnit baseUnit = unitPrefab.GetComponent<BaseUnit>();
        if (baseUnit == null)
        {
            Debug.LogWarning("BaseUnit ������Ʈ�� ���� �����տ� �����ϴ�.");
            return;
        }

        //��� ���� �õ�
        if (!GoldManager.instance.SpendGold(baseUnit.goldCost))
        {
            CancelPlacement(); // ��� ���� �� ��ġ ���
            return;
        }

        //���� ��ġ
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

    // ��� Ÿ�� ����
    public void SetAsPathTile()
    {
        isPathTile = true;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = new Color(0.4f, 0.4f, 0.4f); // ȸ��
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