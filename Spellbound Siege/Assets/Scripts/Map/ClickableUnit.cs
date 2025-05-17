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
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 1.2f;
        currentUI = Instantiate(unitActionUIPrefab, spawnPos, Quaternion.identity);

        // 1. �θ� ����: ���� ȸ�� ���� ����
        currentUI.transform.SetParent(null);

        // 2. ī�޶� ���� �ٶ󺸵��� ȸ�� ����
        if (Camera.main != null)
        {
            // ��Ȯ�� ī�޶� �ٶ󺸴� �������� �ո� ����
            Vector3 cameraForward = Camera.main.transform.forward;
            currentUI.transform.forward = cameraForward;
        }

        // 3. ��ư ��� ����
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
