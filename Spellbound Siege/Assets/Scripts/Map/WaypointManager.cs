using UnityEngine;

/// <summary>
/// Waypoint 들을 기준으로 경로 타일을 자동 생성
/// </summary>
public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;
    public GameObject pathTilePrefab;
    public float tileSize = 1.0f;
    public float tileHeight = 0.01f;

    private void Awake() // 게임 시작 전 생성
    {
        GeneratePathTiles();
    }

    public void GeneratePathTiles()
    {
        if (pathTilePrefab == null || waypoints.Length < 2)
        {
            Debug.LogWarning("PathTile 프리팹 또는 웨이포인트가 부족합니다.");
            return;
        }

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 start = waypoints[i].position;
            Vector3 end = waypoints[i + 1].position;

            start.x = Mathf.Round(start.x / tileSize) * tileSize;
            start.z = Mathf.Round(start.z / tileSize) * tileSize;
            end.x = Mathf.Round(end.x / tileSize) * tileSize;
            end.z = Mathf.Round(end.z / tileSize) * tileSize;
            start.y = end.y = tileHeight;

            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            int steps = Mathf.CeilToInt(distance / tileSize);

            for (int j = 0; j <= steps; j++)
            {
                Vector3 rawPos = start + direction * j * tileSize;
                float snappedX = Mathf.Round(rawPos.x / tileSize) * tileSize;
                float snappedZ = Mathf.Round(rawPos.z / tileSize) * tileSize;
                Vector3 snappedPos = new Vector3(snappedX, tileHeight, snappedZ);

                GameObject tile = Instantiate(pathTilePrefab, snappedPos, Quaternion.identity, transform);

                // 설치 금지 타일 표시
                GridTile gt = tile.GetComponent<GridTile>();
                if (gt != null) gt.SetAsPathTile();
            }

            // 마지막 종착점도 보정해서 한 칸 더 깔기
            Vector3 endSnappedPos = new Vector3(
                Mathf.Round(end.x / tileSize) * tileSize,
                tileHeight,
                Mathf.Round(end.z / tileSize) * tileSize
            );

            GameObject finalTile = Instantiate(pathTilePrefab, endSnappedPos, Quaternion.identity, transform);

            GridTile gtFinal = finalTile.GetComponent<GridTile>();
            if (gtFinal != null) gtFinal.SetAsPathTile();
        }
    }

    public Transform GetWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Length)
            return waypoints[index];
        return null;
    }

    public Vector3[] GetAllWaypointPositions()
    {
        Vector3[] positions = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
            positions[i] = waypoints[i].position;
        return positions;
    }
}