using System.Collections.Generic;
using UnityEngine;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();  // 덱 리스트
    public Transform handTransform;  // UI에서 카드를 표시할 위치
    public GameObject cardUIPrefab;  // 카드 UI 프리팹

    private List<Card> drawPile = new List<Card>(); // 실제 뽑을 카드 목록

    void Start()
    {
        ResetDeck(); // 게임 시작 시 덱을 초기화
        DrawCard(5); // 처음에 5장 뽑기
    }

    public void ResetDeck()
    {
        drawPile = new List<Card>(deck); // 덱 복사
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            Card temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }

    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count == 0) return; // 덱이 비었으면 종료

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            CreateCardUI(drawnCard);
        }
    }

    private void CreateCardUI(Card card)
    {
        GameObject cardObject = Instantiate(cardUIPrefab, handTransform);
        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.SetCard(card);
    }
}