using UnityEngine;

public class UnitPreviewManager : MonoBehaviour  // 클래스 이름 변경
{
    public static UnitPreviewManager instance;
    public GameObject selectedUnit;

    private void Awake()
    {
        instance = this;
    }

    public void SetSelectedUnit(GameObject unit)
    {
        selectedUnit = unit;
    }
}
