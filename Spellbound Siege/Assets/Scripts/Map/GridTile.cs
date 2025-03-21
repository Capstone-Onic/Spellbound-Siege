using UnityEngine;

public class GridTile : MonoBehaviour
{
    public GameObject unitShadowPrefab;  // 그림자 프리팹 (Inspector에서 설정)
    private GameObject currentUnit;      // 배치된 유닛
    private GameObject currentShadow;    // 현재 그림자
    private bool isOccupied = false;     // 타일이 점유되었는지 여부
    private bool isPlacing = false;      // 유닛 배치 중인지 확인
    private Vector3 initialClickPosition;  // 처음 클릭한 위치
    private static readonly float cancelThreshold = 5.0f;  // 이동 취소 기준 거리

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
            // 마우스가 클릭된 후 움직였는지 확인
            if (Vector3.Distance(initialClickPosition, Input.mousePosition) > cancelThreshold)
            {
                CancelPlacement();  // 이동하면 설치 취소
            }
            else if (Input.GetMouseButtonUp(0))  // 클릭을 떼면 설치 확정
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
            initialClickPosition = Input.mousePosition;  // 클릭 위치 저장
        }
    }

    private void PlaceUnit()
    {
        if (currentShadow != null)
        {
            Destroy(currentShadow);  // 그림자를 제거
        }

        currentUnit = Instantiate(UnitManager.instance.selectedUnit, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        isOccupied = true;
        isPlacing = false;
    }

    private void CancelPlacement()
    {
        if (currentShadow != null)
        {
            Destroy(currentShadow);  // 그림자 제거
        }

        isPlacing = false;
    }
}
