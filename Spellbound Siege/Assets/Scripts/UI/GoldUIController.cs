using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldUIController : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    private int displayedGold = 0;
    public Image coinIcon;
    private bool isAnimating = false;

    private void Start()
    {
        UpdateGoldInstantly(); // ���� �� ������
    }
    public void UpdateGold(int value)
    {
        goldText.text = $"{value}";
    }

    private void Update()
    {
        int targetGold = GoldManager.instance.currentGold;

        if (displayedGold != targetGold)
        {
            float speed = 200f;
            displayedGold = Mathf.RoundToInt(Mathf.MoveTowards(displayedGold, targetGold, speed * Time.deltaTime));
            goldText.text = $"{displayedGold}";

            if (!isAnimating)
            {
                isAnimating = true;

                LeanTween.cancel(coinIcon.gameObject);
                coinIcon.transform.localScale = Vector3.one * 1.08f;
                LeanTween.scale(coinIcon.gameObject, Vector3.one, 0.2f)
                    .setEaseOutBack()
                    .setOnComplete(() => isAnimating = false); // �Ϸ� �� �ٽ� ���� ����
            }
        }
    }

    // ��� ������Ʈ (�� ���� ��)
    public void UpdateGoldInstantly()
    {
        displayedGold = GoldManager.instance.currentGold;
        goldText.text = $"{displayedGold}";
    }
}
