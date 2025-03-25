using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    private Card cardData;
    private CardDrawManager drawManager; // ī�� �Ŵ��� ����
    public Image cardImage;              // ī�� �̹����� ǥ���� Image ������Ʈ


    public void SetCard(Card card, CardDrawManager manager)
    {
        cardData = card;
        cardNameText.text = card.cardName;
        drawManager = manager;
    }

    public void OnCardClick()
    {
        UseCard();
    }

    private void UseCard()
    {
        Debug.Log($"ī�� ���: {cardData.cardName}");

        // ���⿡ ī�� ȿ�� ���� �߰� (��: ���� ��ȯ, ���� ��)

        drawManager.RemoveCardFromHand(this.gameObject); // �տ��� ī�� ����
    }
}