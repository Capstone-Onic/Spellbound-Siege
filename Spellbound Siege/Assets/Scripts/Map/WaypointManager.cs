using UnityEngine;

/// <summary>
/// Waypoint ���� �������� ������ ������ �� �ִ� ��θ� �����ϰ�,
/// �ش� ��� ���� PathTile �������� �ڵ����� ��ġ�ϴ� ��ũ��Ʈ
/// </summary>
public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints;             // ���̾��Ű�� �ִ� WayPoint1~4 ���� ������Ʈ��
    public GameObject pathTilePrefab;         // ��� Ÿ�� ������
    public float tileSize = 1.0f;             // Ÿ�� ���� (�׸��� ������ ����)
    public float tileHeight = 0.01f;          // �ٴں��� ��¦ ���� ���� (��ħ ������)

    private void Start()
    {
        // ���� ���� �� �ڵ����� ��� Ÿ�� ����
        GeneratePathTiles();
    }

    /// <summary>
    /// WayPoint�� ���� ��θ� ���� Ÿ���� ����
    /// �� WayPoint ���̿� ���� ��θ� ����� �� ���� PathTile�� ��ġ
    /// </summary>
    public void GeneratePathTiles()
    {
        if (pathTilePrefab == null || waypoints.Length < 2)
        {
            Debug.LogWarning("PathTile ������ �Ǵ� ��������Ʈ�� �����մϴ�.");
            return;
        }

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            // �������� ���� �޾ƿ���
            Vector3 start = waypoints[i].position;
            Vector3 end = waypoints[i + 1].position;

            // �������� ������ �׸��� ������ ��Ȯ�� ���� (0.5�� �Ҽ��� ���� ���� ����)
            start.x = Mathf.Round(start.x / tileSize) * tileSize;
            start.z = Mathf.Round(start.z / tileSize) * tileSize;
            end.x = Mathf.Round(end.x / tileSize) * tileSize;
            end.z = Mathf.Round(end.z / tileSize) * tileSize;
            start.y = end.y = tileHeight; // ���� ����

            // ���� ���� ��� (����ȭ)
            Vector3 direction = (end - start).normalized;

            // �� �Ÿ� ���� �� Ÿ�� ���� ���
            float distance = Vector3.Distance(start, end);
            int steps = Mathf.CeilToInt(distance / tileSize);

            for (int j = 0; j <= steps; j++)
            {
                // ���� Ÿ�� ��ġ ���
                Vector3 rawPos = start + direction * j * tileSize;

                // ��ġ�� Ÿ�� ���ڿ� ���� ����
                float snappedX = Mathf.Round(rawPos.x / tileSize) * tileSize;
                float snappedZ = Mathf.Round(rawPos.z / tileSize) * tileSize;
                Vector3 snappedPos = new Vector3(snappedX, tileHeight, snappedZ);

                // Ÿ�� ������ ����
                Instantiate(pathTilePrefab, snappedPos, Quaternion.identity, transform);
            }

            // ������ ������ ��Ȯ�� �� �� �� ���� (Ȥ�� �� ���� ��� ���)
            float finalX = Mathf.Round(end.x / tileSize) * tileSize;
            float finalZ = Mathf.Round(end.z / tileSize) * tileSize;
            Vector3 endSnappedPos = new Vector3(finalX, tileHeight, finalZ);
            Instantiate(pathTilePrefab, endSnappedPos, Quaternion.identity, transform);
        }
    }
}
