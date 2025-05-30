using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSelectionUI : MonoBehaviour
{
    [Header("유닛 프리팹 목록")]
    public GameObject[] unitPrefabs;

    [Header("버튼 프리팹 (Button + Icon + NameText + CostText + Outline)")]
    public GameObject unitButtonPrefab;

    [Header("버튼이 생성될 부모 (Horizontal Layout Group 적용된 오브젝트)")]
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

        // 기존 버튼 제거
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
                Debug.LogWarning($"{prefab.name}에 BaseUnit 컴포넌트가 없습니다.");
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
