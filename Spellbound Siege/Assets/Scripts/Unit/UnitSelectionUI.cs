using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSelectionUI : MonoBehaviour
{
    [Header("���� ������ ���")]
    public GameObject[] unitPrefabs;

    [Header("��ư ������ (Button + Icon + NameText + CostText + Outline)")]
    public GameObject unitButtonPrefab;

    [Header("��ư�� ������ �θ� (Horizontal Layout Group ����� ������Ʈ)")]
    public Transform buttonContainer;

    private List<GameObject> selectedUnits = new List<GameObject>();
    private ButtonHighlighter currentSelected;

    void Start()
    {
        GenerateOrderedUnitButtons();
    }

    void GenerateOrderedUnitButtons()
    {
        selectedUnits.Clear();

        // ���� ��ư ����
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        int maxCount = Mathf.Min(5, unitPrefabs.Length);

        for (int i = 0; i < maxCount; i++)
        {
            GameObject prefab = unitPrefabs[i];
            selectedUnits.Add(prefab);

            BaseUnit unit = prefab.GetComponent<BaseUnit>();
            if (unit == null)
            {
                Debug.LogWarning($"{prefab.name}�� BaseUnit ������Ʈ�� �����ϴ�.");
                continue;
            }

            GameObject button = Instantiate(unitButtonPrefab, buttonContainer);
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            button.transform.Find("Icon").GetComponent<Image>().sprite = unit.unitIcon;
            button.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = unit.unitName;
            button.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = $"{unit.goldCost}";

            ButtonHighlighter highlighter = button.GetComponent<ButtonHighlighter>();
            highlighter.SetHighlighted(false);

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentSelected != null)
                    currentSelected.SetHighlighted(false);

                currentSelected = highlighter;
                currentSelected.SetHighlighted(true);

                UnitManager.instance.SetSelectedUnit(prefab);
                SFXManager.Instance?.PlaySelect();
            });
        }
    }

}
