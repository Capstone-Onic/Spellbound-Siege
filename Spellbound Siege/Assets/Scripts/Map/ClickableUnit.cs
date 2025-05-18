using UnityEngine;

public class ClickableUnit : MonoBehaviour
{
    private GridTile parentTile;
    private BaseUnit baseUnit;

    [Header("���� �׼� UI ������")]
    public GameObject unitActionUIPrefab;
    private GameObject currentUI;

    public void SetParentTile(GridTile tile)
    {
        parentTile = tile;
    }

    private void Start()
    {
        baseUnit = GetComponent<BaseUnit>();
    }

    private void OnMouseDown()
    {
        if (!StartGameManager.isPlacementPhase) return;

        ToggleUnitUI();
    }

    private void ToggleUnitUI()
    {
        if (UIManager.Instance.IsUIOpen)
        {
            // ���� ���� �ٽ� ������ ��� �ݱ�
            if (UIManager.Instance.currentUnitUI == currentUI)
            {
                UIManager.Instance.CloseUI();
                return;
            }

            // �ٸ� UI�� ���� ������ �ݱ�
            UIManager.Instance.CloseUI();
        }

        Renderer rend = GetComponentInChildren<Renderer>();
        float unitHeight = rend != null ? rend.bounds.size.y : 2f;
        Vector3 spawnPos = transform.position + Vector3.up * (unitHeight + 0.2f);

        currentUI = Instantiate(unitActionUIPrefab, spawnPos, Quaternion.identity);
        currentUI.transform.SetParent(null);
        UIManager.Instance.OpenUI(currentUI); // UI ���

        UnitActionUI ui = currentUI.GetComponent<UnitActionUI>();
        if (ui != null)
        {
            ui.Setup(this);
        }
    }


    public void SellUnit()
    {
        if (baseUnit != null)
        {
            int refund = baseUnit.goldCost / 2;
            GoldManager.instance.AddGold(refund);
        }

        if (parentTile != null)
        {
            parentTile.ClearUnit();
            parentTile = null;
        }

        Destroy(gameObject);
    }

    public void UpgradeUnit()
    {
        int upgradeCost = 10; // ���� ����

        if (GoldManager.instance.SpendGold(upgradeCost))
        {
            Debug.Log("[��ȭ] ���� ��ȭ �Ϸ�");

            baseUnit.IncreaseStats(); // ����/���� ��ȭ�� �Լ� (���� ���� ���� ����)
        }
        else
        {
            Debug.Log("[��ȭ] ��� �������� ��ȭ ����");
        }
    }
}
