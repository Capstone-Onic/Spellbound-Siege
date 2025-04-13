using UnityEngine;
using TMPro;

public class GoldUIController : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    private int displayedGold = 0;

    private void Start()
    {
        UpdateGoldInstantly(); // ���� �� ������
    }

    private void Update()
    {
        // ���� ���� ǥ�õ� ��尡 �ٸ��� �ε巴�� ����
        int targetGold = GoldManager.instance.currentGold;
        if (displayedGold < targetGold)
        {
            displayedGold += Mathf.CeilToInt((targetGold - displayedGold) * Time.deltaTime * 8f); // �ε巴�� ����
            if (displayedGold > targetGold)
                displayedGold = targetGold;

            goldText.text = $"{displayedGold}";
        }
    }

    // ��� ������Ʈ (�� ���� ��)
    public void UpdateGoldInstantly()
    {
        displayedGold = GoldManager.instance.currentGold;
        goldText.text = $"{displayedGold}";
    }
}
