using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();  // 덱 리스트
    public Transform handTransform;  // UI에서 카드를 표시할 위치
    public GameObject cardUIPrefab;  // 카드 UI 프리팹

    private List<Card> drawPile = new List<Card>(); // 실제 뽑을 카드 목록

    void Start()
    {
        ResetDeck(); // 게임 시작 시 덱을 초기화
        Debug.Log($"덱 개수: {drawPile.Count}");
        DrawCard(5); // 처음에 5장 뽑기
        StartCoroutine(AutoDraw()); // 자동 드로우 시작
    }

    public void ResetDeck()
    {
        drawPile = new List<Card>(deck); // 덱 복사
        Debug.Log($"초기화 후 덱 개수: {drawPile.Count}");
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
        Debug.Log($"DrawCard 호출! 요청된 카드 수: {count}, 현재 덱 개수: {drawPile.Count}");

        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count == 0)
            {
                Debug.LogWarning("덱이 비었습니다! 더 이상 카드를 뽑을 수 없음.");
                return;
            }

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            Debug.Log($"카드 {i + 1}번째 뽑기: {drawnCard.cardName}, 남은 덱 개수: {drawPile.Count}");
            CreateCardUI(drawnCard);
        }
    }


    private void CreateCardUI(Card card)
    {
        GameObject cardObject = Instantiate(cardUIPrefab, handTransform);
        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.SetCard(card, this);

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            int cardIndex = handTransform.childCount - 1;
            rectTransform.anchoredPosition = new Vector2(150 * cardIndex, 0); // 카드 간격 조정
        }
    }

    public void RemoveCardFromHand(GameObject cardObject)
    {
        Destroy(cardObject); // 카드 제거
    }

    private IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10초 대기
            DrawCard(1); // 카드 1장 드로우
        }
    }
}