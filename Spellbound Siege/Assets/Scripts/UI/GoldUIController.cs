using UnityEngine;
using TMPro;

public class GoldUIController : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    private int displayedGold = 0;

    private void Start()
    {
        UpdateGoldInstantly(); // 시작 시 맞춰줌
    }

    private void Update()
    {
        // 현재 골드와 표시된 골드가 다르면 부드럽게 증가
        int targetGold = GoldManager.instance.currentGold;
        if (displayedGold < targetGold)
        {
            displayedGold += Mathf.CeilToInt((targetGold - displayedGold) * Time.deltaTime * 8f); // 부드럽게 증가
            if (displayedGold > targetGold)
                displayedGold = targetGold;

            goldText.text = $"{displayedGold}";
        }
    }

    // 즉시 업데이트 (씬 시작 시)
    public void UpdateGoldInstantly()
    {
        displayedGold = GoldManager.instance.currentGold;
        goldText.text = $"{displayedGold}";
    }
}
