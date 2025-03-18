using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    private Card cardData;

    public void SetCard(Card card)
    {
        cardData = card;
        cardNameText.text = card.cardName;
    }
}