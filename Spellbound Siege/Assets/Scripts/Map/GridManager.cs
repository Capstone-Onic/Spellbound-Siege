using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10;                // 그리드 가로 크기
    public int height = 10;               // 그리드 세로 크기
    public float tileSize = 1.0f;         // 타일 간격 (크기)

    public GameObject tilePrefab;         // 내부 타일 프리팹 (GroundTile)
    public GameObject outerTilePrefab;    // 외곽 타일 프리팹 (잔디)
    public GameObject treePrefab;         // 외곽에 배치할 나무 프리팹

    public int borderThickness = 3;       // 외곽 테두리 타일 범위
    public float treeOffset = 1.0f;       // 나무가 바닥에서 얼마나 떨어질지 거리

    private void Start()
    {
        GenerateGrid();                   // 내부 바닥 타일 생성
        GenerateOuterTilesAndTrees();     // 외곽 타일 및 나무 생성
        ApplyRenderingFixes();            // 렌더링 설정 조정 (조명, 인스턴싱 해제 등)
    }

    void GenerateGrid()
    {
        // 가로, 세로 반복하면서 내부 타일 배치
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                Instantiate(tilePrefab, position, Quaternion.identity, transform); // 타일 생성
            }
        }
    }

    void GenerateOuterTilesAndTrees()
    {
        // 그리드 범위 바깥까지 반복
        for (int x = -borderThickness; x < width + borderThickness; x++)
        {
            for (int z = -borderThickness; z < height + borderThickness; z++)
            {
                bool isInside = (x >= 0 && x < width && z >= 0 && z < height);
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                // 내부가 아니면 외곽 타일 생성
                if (!isInside && outerTilePrefab != null)
                {
                    Instantiate(outerTilePrefab, position, Quaternion.identity, transform);
                }

                // 외곽에 나무 배치
                if (!isInside && treePrefab != null)
                {
                    Vector3 directionFromCenter = position - GetGridCenter(); // 중심에서 방향 계산
                    Vector3 offsetDirection = directionFromCenter.normalized * treeOffset; // 일정 거리만큼 밀어냄
                    Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f)); // 자연스럽게 흔들기
                    Vector3 finalPos = position + offsetDirection + randomOffset;

                    GameObject tree = Instantiate(treePrefab, finalPos, Quaternion.identity, transform);

                    tree.transform.Rotate(0, Random.Range(0f, 360f), 0); // 회전 랜덤
                    float scale = Random.Range(0.9f, 1.2f);               // 크기 랜덤
                    tree.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }

    private Vector3 GetGridCenter()
    {
        float totalWidth = width * tileSize;
        float totalHeight = height * tileSize;
        return transform.position + new Vector3(totalWidth / 2f - tileSize / 2f, 0f, totalHeight / 2f - tileSize / 2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // 연한 초록색 반투명 박스
        float totalWidth = width * tileSize;
        float totalHeight = height * tileSize;
        Vector3 center = GetGridCenter();
        Gizmos.DrawCube(center, new Vector3(totalWidth, 0f, totalHeight)); // 씬 뷰에 그리드 영역 미리보기
    }
    
   private void ApplyRenderingFixes()
    {
        // 타일 관련 머티리얼 전부 찾아서 인스턴싱 해제
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        foreach (Material mat in materials)
        {
            if (mat.name.Contains("Tile") || mat.name.Contains("Grass"))
            {
                mat.enableInstancing = false; // GPU 인스턴싱 끄기
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack; // 전역 조명 영향 제거
            }
        }
    }
}
