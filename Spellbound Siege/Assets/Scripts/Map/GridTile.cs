using UnityEngine;

public class GridTile : MonoBehaviour
{
    public GameObject unitShadowPrefab;  // �׸��� ������ (Inspector���� ����)
    private GameObject currentUnit;      // ��ġ�� ����
    private GameObject currentShadow;    // ���� �׸���
    private bool isOccupied = false;     // Ÿ���� �����Ǿ����� ����
    private bool isPlacing = false;      // ���� ��ġ ������ Ȯ��
    private Vector3 initialClickPosition;  // ó�� Ŭ���� ��ġ
    private static readonly float cancelThreshold = 5.0f;  // �̵� ��� ���� �Ÿ�

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
            // ���콺�� Ŭ���� �� ���������� Ȯ��
            if (Vector3.Distance(initialClickPosition, Input.mousePosition) > cancelThreshold)
            {
                CancelPlacement();  // �̵��ϸ� ��ġ ���
            }
            else if (Input.GetMouseButtonUp(0))  // Ŭ���� ���� ��ġ Ȯ��
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
            initialClickPosition = Input.mousePosition;  // Ŭ�� ��ġ ����
        }
    }

    private void PlaceUnit()
    {
        if (currentShadow != null)
        {
            Destroy(currentShadow);  // �׸��ڸ� ����
        }

        currentUnit = Instantiate(UnitManager.instance.selectedUnit, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        isOccupied = true;
        isPlacing = false;
    }

    private void CancelPlacement()
    {
        if (currentShadow != null)
        {
            Destroy(currentShadow);  // �׸��� ����
        }

        isPlacing = false;
    }
}
