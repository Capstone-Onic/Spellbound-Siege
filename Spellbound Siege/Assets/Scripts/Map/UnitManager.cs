using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance; // 싱글톤 패턴으로 사용하기 위함
    public GameObject selectedUnit;     // 배치할 유닛 (Inspector에서 지정)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedUnit(GameObject unit)
    {
        selectedUnit = unit;
    }
}
