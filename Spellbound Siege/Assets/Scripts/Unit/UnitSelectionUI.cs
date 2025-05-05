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
        GenerateRandomUnitButtons();
    }

    void GenerateRandomUnitButtons()
    {
        selectedUnits.Clear();

        // ���� ��ư ����
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        int maxCount = Mathf.Min(5, unitPrefabs.Length);
        List<GameObject> shuffled = unitPrefabs.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject prefab = shuffled[i];
            selectedUnits.Add(prefab);

            BaseUnit unit = prefab.GetComponent<BaseUnit>();
            if (unit == null)
            {
                Debug.LogWarning($"{prefab.name}�� BaseUnit ������Ʈ�� �����ϴ�.");
                continue;
            }

            // ��ư ���� �� ����
            GameObject button = Instantiate(unitButtonPrefab, buttonContainer);
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            // UI ��� ����
            button.transform.Find("Icon").GetComponent<Image>().sprite = unit.unitIcon;
            button.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = unit.unitName;
            button.transform.Find("CostText").GetComponent<TextMeshProUGUI>().text = $"{unit.goldCost}";

            // ���� ������Ʈ
            ButtonHighlighter highlighter = button.GetComponent<ButtonHighlighter>();
            highlighter.SetHighlighted(false);

            // Ŭ�� �̺�Ʈ ���
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                // ���� ���� ����
                if (currentSelected != null)
                    currentSelected.SetHighlighted(false);

                // �� ���� ����
                currentSelected = highlighter;
                currentSelected.SetHighlighted(true);

                // ���� ����
                UnitManager.instance.SetSelectedUnit(prefab);
            });
        }
    }
}
