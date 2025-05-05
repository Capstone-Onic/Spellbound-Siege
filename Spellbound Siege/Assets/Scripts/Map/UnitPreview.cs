using TMPro;
using UnityEngine;

public class UnitPreviewManager : MonoBehaviour  // 클래스 이름 변경
{
    public static UnitPreviewManager instance;
    public GameObject selectedUnit;
    public TextMeshProUGUI costText;

    private void Awake()
    {
        instance = this;
    }

    public void SetSelectedUnit(GameObject unit)
    {
        selectedUnit = unit;
    }
    public void SetPreview(GameObject unitPrefab)
    {
        BaseUnit baseUnit = unitPrefab.GetComponent<BaseUnit>();
        if (baseUnit != null)
        {
            costText.text = $"{baseUnit.goldCost}";
        }
    }
}
