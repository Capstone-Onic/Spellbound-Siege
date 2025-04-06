using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float tileSize = 1.0f;

    public GameObject tilePrefab;
    public GameObject outerTilePrefab;
    public GameObject treePrefab;

    public int borderThickness = 3;
    public float treeOffset = 1.0f;

    private void Start()
    {
        GenerateGrid();
        GenerateOuterTilesAndTrees();
        ApplyRenderingFixes();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                Instantiate(tilePrefab, position, Quaternion.identity, transform);
            }
        }
    }

    void GenerateOuterTilesAndTrees()
    {
        for (int x = -borderThickness; x < width + borderThickness; x++)
        {
            for (int z = -borderThickness; z < height + borderThickness; z++)
            {
                bool isInside = (x >= 0 && x < width && z >= 0 && z < height);
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                if (!isInside && outerTilePrefab != null)
                {
                    Instantiate(outerTilePrefab, position, Quaternion.identity, transform);
                }

                if (!isInside && treePrefab != null)
                {
                    Vector3 directionFromCenter = position - GetGridCenter();
                    Vector3 offsetDirection = directionFromCenter.normalized * treeOffset;
                    Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
                    Vector3 finalPos = position + offsetDirection + randomOffset;

                    GameObject tree = Instantiate(treePrefab, finalPos, Quaternion.identity, transform);
                    tree.transform.Rotate(0, Random.Range(0f, 360f), 0);
                    float scale = Random.Range(0.9f, 1.2f);
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
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        float totalWidth = width * tileSize;
        float totalHeight = height * tileSize;
        Vector3 center = GetGridCenter();
        Gizmos.DrawCube(center, new Vector3(totalWidth, 0.1f, totalHeight));
    }

    private void ApplyRenderingFixes()
    {
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        foreach (Material mat in materials)
        {
            if (mat.name.Contains("Tile") || mat.name.Contains("Grass"))
            {
                mat.enableInstancing = false;
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }
    }
}
