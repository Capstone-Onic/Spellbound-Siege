using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Spellbound;

public class CardDrawManager : MonoBehaviour
{
    // 카드 덱 (초기 카드 리스트)
    public List<Card> deck = new List<Card>();

    // 카드가 배치될 손 패널 (UI 상의 위치)
    public RectTransform handPanel;

    // 카드 프리팹 (UI 카드 오브젝트)
    public GameObject cardPrefab;

    // 최대 손패 크기
    public int maxHandSize = 8;

    // 카드가 나올 위치(덱 위치)
    public RectTransform deckTransform;

    // 카드 드로우 애니메이션 시간
    public float drawDuration = 0.5f;

    // 실제 드로우에 사용할 카드 더미
    private List<Card> drawPile = new List<Card>();

    // 현재 손에 있는 카드 오브젝트들
    private List<GameObject> handCards = new List<GameObject>();

    // 시작 시 덱을 초기화하고, 초기 카드 5장 드로우 및 자동 드로우 시작
    void Start()
    {
        ResetDeck();
        DrawCard(5);
        StartCoroutine(AutoDraw());
    }

    // 덱 초기화 (드로우 덱에 카드 복사 + 셔플)
    public void ResetDeck()
    {
        drawPile = new List<Card>(deck);
        ShuffleDeck();
    }

    // 덱 셔플
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

    // 지정된 수만큼 카드를 드로우
    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 드로우할 카드가 없거나 손이 가득 찼을 경우 중단
            if (drawPile.Count == 0 || handCards.Count >= maxHandSize)
                return;

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            StartCoroutine(CreateCardUI(drawnCard)); // 카드 UI 생성 및 애니메이션 처리
        }
    }

    // 카드 UI를 생성하고 덱에서 손 위치로 이동하는 코루틴
    private IEnumerator CreateCardUI(Card card)
    {
        // 1. 파괴된 카드 정리
        handCards.RemoveAll(cardObj => cardObj == null);

        if (deckTransform == null || handPanel == null) yield break;

        // 2. 새 카드 생성
        GameObject cardObject = Instantiate(cardPrefab, handPanel);
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        cardDisplay.SetCard(card);

        // 3. 덱 위치에서 시작
        Vector2 startPosition = deckTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;

        // 4. 카드 리스트에 추가
        handCards.Add(cardObject);

        // 5. 카드 수에 따른 위치 계산
        List<Vector2> positions = CalculateCardPositions(handCards.Count);

        // 6. 이동 애니메이션 수행
        float elapsedTime = 0f;
        while (elapsedTime < drawDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawDuration);

            int count = Mathf.Min(handCards.Count, positions.Count);
            for (int i = 0; i < count; i++)
            {
                if (handCards[i] == null) continue;

                RectTransform rt = handCards[i].GetComponent<RectTransform>();
                if (rt == null) continue;

                Vector2 target = positions[i];
                rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, target, t);
            }

            yield return null;
        }

        // 7. 정렬 마무리 (위치 고정)
        int finalCount = Mathf.Min(handCards.Count, positions.Count);
        for (int i = 0; i < finalCount; i++)
        {
            if (handCards[i] == null) continue;

            RectTransform rt = handCards[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = positions[i];
        }
    }

    // 카드 수에 맞게 손패에서 위치 정렬 계산
    private List<Vector2> CalculateCardPositions(int cardCount)
    {
        List<Vector2> positions = new List<Vector2>();

        float cardY = -25f; // Y 위치 고정

        if (cardCount == 0) return positions;

        // 카드 간 최대 간격 제한 (겹치게 만들기 위한 설정)
        float maxSpacing = 160f; // spacing을 줄이려면 이 값 줄이기
        float spacing = Mathf.Min(maxSpacing, handPanel.rect.width / cardCount);

        // 전체 카드 너비
        float totalWidth = spacing * (cardCount - 1);
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            float x = startX + i * spacing;
            positions.Add(new Vector2(x, cardY));
        }

        return positions;
    }

    // 손에서 카드 제거 및 정렬 갱신
    public void RemoveCardFromHand(GameObject cardObject)
    {
        if (handCards.Contains(cardObject))
        {
            handCards.Remove(cardObject);
            Destroy(cardObject); // ← 제거는 마지막에
        }
        ReorganizeHand();
    }

    // 손에 있는 카드들의 위치 재배치
    private void ReorganizeHand()
    {
        List<Vector2> positions = CalculateCardPositions(handCards.Count);

        for (int i = 0; i < handCards.Count; i++)
        {
            RectTransform rectTransform = handCards[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = positions[i];
            }
        }
    }

    // 일정 시간마다 자동으로 카드 드로우
    private IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5초마다
            DrawCard(1);
        }
    }
}