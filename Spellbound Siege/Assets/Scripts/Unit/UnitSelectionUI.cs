using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionUI : MonoBehaviour
{
    public GameObject meleeUnitPrefab;
    public GameObject rangedUnitPrefab;

    public Button meleeButton;
    public Button rangedButton;

    void Start()
    {
        meleeButton.onClick.AddListener(() =>
        {
            UnitManager.instance.SetSelectedUnit(meleeUnitPrefab);
        });

        rangedButton.onClick.AddListener(() =>
        {
            UnitManager.instance.SetSelectedUnit(rangedUnitPrefab);
        });
    }
}
