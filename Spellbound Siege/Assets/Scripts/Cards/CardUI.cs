using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TextMeshProUGUI cardNameText;
    private Card cardData;
    private CardDrawManager drawManager; // 카드 매니저 참조
    public Image cardImage;              // 카드 이미지를 표시할 Image 컴포넌트


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
        Debug.Log($"카드 사용: {cardData.cardName}");

        // 여기에 카드 효과 로직 추가 (예: 유닛 소환, 공격 등)

        drawManager.RemoveCardFromHand(this.gameObject); // 손에서 카드 제거
    }
}