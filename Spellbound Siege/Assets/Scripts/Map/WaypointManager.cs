using UnityEngine;

/// <summary>
/// Waypoint 들을 기준으로 유닛이 지나갈 수 있는 경로를 생성하고,
/// 해당 경로 위에 PathTile 프리팹을 자동으로 배치하는 스크립트
/// </summary>
public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;             // 하이어라키에 있는 WayPoint1~4 같은 오브젝트들
    public GameObject pathTilePrefab;         // 경로 타일 프리팹
    public float tileSize = 1.0f;             // 타일 간격 (그리드 단위와 같게)
    public float tileHeight = 0.01f;          // 바닥보다 살짝 위에 띄우기 (겹침 방지용)

    private void Start()
    {
        // 게임 시작 시 자동으로 경로 타일 생성
        GeneratePathTiles();
    }

    /// <summary>
    /// WayPoint들 간의 경로를 따라 타일을 생성
    /// 각 WayPoint 사이에 직선 경로를 만들고 그 위에 PathTile을 배치
    /// </summary>
    public void GeneratePathTiles()
    {
        if (pathTilePrefab == null || waypoints.Length < 2)
        {
            Debug.LogWarning("PathTile 프리팹 또는 웨이포인트가 부족합니다.");
            return;
        }

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            // 시작점과 끝점 받아오기
            Vector3 start = waypoints[i].position;
            Vector3 end = waypoints[i + 1].position;

            // 시작점과 끝점을 그리드 단위로 정확히 스냅 (0.5나 소수점 단위 오차 방지)
            start.x = Mathf.Round(start.x / tileSize) * tileSize;
            start.z = Mathf.Round(start.z / tileSize) * tileSize;
            end.x = Mathf.Round(end.x / tileSize) * tileSize;
            end.z = Mathf.Round(end.z / tileSize) * tileSize;
            start.y = end.y = tileHeight; // 높이 고정

            // 방향 벡터 계산 (정규화)
            Vector3 direction = (end - start).normalized;

            // 총 거리 측정 후 타일 개수 계산
            float distance = Vector3.Distance(start, end);
            int steps = Mathf.CeilToInt(distance / tileSize);

            for (int j = 0; j <= steps; j++)
            {
                // 현재 타일 위치 계산
                Vector3 rawPos = start + direction * j * tileSize;

                // 위치를 타일 격자에 맞춰 스냅
                float snappedX = Mathf.Round(rawPos.x / tileSize) * tileSize;
                float snappedZ = Mathf.Round(rawPos.z / tileSize) * tileSize;
                Vector3 snappedPos = new Vector3(snappedX, tileHeight, snappedZ);

                // 타일 프리팹 생성
                Instantiate(pathTilePrefab, snappedPos, Quaternion.identity, transform);
            }

            // 마지막 지점에 명확히 한 번 더 생성 (혹시 안 맞을 경우 대비)
            float finalX = Mathf.Round(end.x / tileSize) * tileSize;
            float finalZ = Mathf.Round(end.z / tileSize) * tileSize;
            Vector3 endSnappedPos = new Vector3(finalX, tileHeight, finalZ);
            Instantiate(pathTilePrefab, endSnappedPos, Quaternion.identity, transform);
        }
    }
}
