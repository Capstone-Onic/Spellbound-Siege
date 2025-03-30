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
        if (deckTransform == null || handPanel == null) yield break;

        int finalCardCount = handCards.Count + 1; // 새 카드 포함 수
        List<Vector2> targetPositions = CalculateCardPositions(finalCardCount); // 정렬 위치 계산

        GameObject cardObject = Instantiate(cardPrefab, handPanel); // 카드 오브젝트 생성
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        cardDisplay.SetCard(card); // 카드 데이터 적용

        // 카드 시작 위치: 덱 위치
        Vector2 startPosition = deckTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;

        // 카드 목표 위치: 마지막 인덱스 위치
        Vector2 targetPosition = targetPositions[finalCardCount - 1];
        handCards.Add(cardObject); // 손패에 추가

        // 애니메이션 처리 (덱 → 손 위치로 이동)
        float elapsedTime = 0f;
        while (elapsedTime < drawDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // 카드 전체 정렬 갱신
        ReorganizeHand();
    }

    // 카드 수에 맞게 손패에서 위치 정렬 계산
    private List<Vector2> CalculateCardPositions(int cardCount)
    {
        List<Vector2> positions = new List<Vector2>();

        float panelWidth = handPanel.rect.width;
        float cardY = 50f; // 카드의 Y 고정값

        if (cardCount == 0) return positions;

        if (cardCount == 1)
        {
            // 카드가 한 장이면 중앙 배치
            positions.Add(new Vector2(0f, cardY));
            return positions;
        }

        float totalWidth = panelWidth;
        float leftEdge = -totalWidth / 2f;
        float spacing = totalWidth / (cardCount - 1); // 카드 간 간격

        for (int i = 0; i < cardCount; i++)
        {
            float x = leftEdge + i * spacing;
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
            Destroy(cardObject);
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