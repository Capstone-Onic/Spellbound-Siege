using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10;          // 가로 타일 개수
    public int height = 10;         // 세로 타일 개수
    public float tileSize = 10f;    // 타일의 크기 (10으로 설정)

    // 📌 여기서 중요한 부분! Tile Prefab 슬롯 만들기
    public GameObject tilePrefab;   // 타일로 사용할 프리팹 (Inspector에서 연결할 수 있음)

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                // Tile Prefab을 배치하기
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.name = $"Tile_{x}_{z}";
                tile.transform.parent = transform;
            }
        }
    }
}
