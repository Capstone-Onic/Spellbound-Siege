using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spellbound;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    private Card cardData;
    private CardDrawManager drawManager;
    public Image cardImage;


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


        drawManager.RemoveCardFromHand(this.gameObject);
    }
}