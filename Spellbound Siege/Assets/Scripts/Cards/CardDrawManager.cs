using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;
using UnityEngine.UI;

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

    void Start()
    {
        if (handPanel != null)
            handPanel.gameObject.SetActive(false);
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
            if (drawPile.Count == 0 || handCards.Count >= maxHandSize)
                return;

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            StartCoroutine(CreateCardUI(drawnCard));
        }
    }

    // 카드 UI를 생성하고 덱에서 손 위치로 이동하는 코루틴
    private IEnumerator CreateCardUI(Card card)
    {
        // 손패에서 파괴된 카드 제거
        handCards.RemoveAll(cardObj => cardObj == null);

        if (deckTransform == null || handPanel == null)
            yield break;

        // 새 카드 생성
        GameObject cardObject = Instantiate(cardPrefab, handPanel);
        RectTransform rt = cardObject.GetComponent<RectTransform>();
        CardDisplay cd = cardObject.GetComponent<CardDisplay>();
        cd.SetCard(card);

        // CanvasGroup이 없으면 추가
        CanvasGroup cg = cardObject.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = cardObject.AddComponent<CanvasGroup>();

        // 확대 중인 카드가 하나라도 있다면 마우스 반응 비활성화
        bool anyCardZoomed = false;
        foreach (var zoom in FindObjectsOfType<CardHoldZoom>())
        {
            if (zoom.IsZooming)
            {
                anyCardZoomed = true;
                break;
            }
        }
        if (anyCardZoomed)
        {
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        // 덱 위치에서 시작 위치 지정
        Vector2 startPos = deckTransform.anchoredPosition;
        rt.anchoredPosition = startPos;

        // 리스트에 추가
        handCards.Add(cardObject);

        // 모든 카드 위치 계산
        List<GameObject> activeCards = new List<GameObject>(handCards);
        List<Vector2> positions = CalculateCardPositions(activeCards);

        // 애니메이션 이동
        float elapsed = 0f;
        while (elapsed < drawDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / drawDuration);

            for (int i = 0; i < activeCards.Count && i < positions.Count; i++)
            {
                GameObject obj = activeCards[i];
                if (obj == null) continue;
                RectTransform cardRt = obj.GetComponent<RectTransform>();
                if (cardRt == null) continue;
                cardRt.anchoredPosition = Vector2.Lerp(cardRt.anchoredPosition, positions[i], t);
            }

            yield return null;
        }

        // 최종 위치 고정
        for (int i = 0; i < activeCards.Count && i < positions.Count; i++)
        {
            GameObject obj = activeCards[i];
            if (obj == null) continue;
            RectTransform cardRt = obj.GetComponent<RectTransform>();
            if (cardRt == null) continue;
            cardRt.anchoredPosition = positions[i];
        }
    }

    // 카드 수에 맞게 손패에서 위치 정렬 계산
    private List<Vector2> CalculateCardPositions(List<GameObject> activeCards)
    {
        List<Vector2> positions = new List<Vector2>();
        int cardCount = activeCards.Count;
        if (cardCount == 0) return positions;

        float cardY = -25f;
        float maxSpacing = 160f;
        float spacing = Mathf.Min(maxSpacing, handPanel.rect.width / cardCount);
        float minSpacing = 90f;
        spacing = Mathf.Clamp(spacing, minSpacing, maxSpacing);

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
            Destroy(cardObject);
        }
        ReorganizeHand();
    }

    // 손에 있는 카드들의 위치 재배치
    public void ReorganizeHand(bool includeAllCards = false)
    {
        List<GameObject> cardsToSort = includeAllCards
            ? handCards
            : handCards.FindAll(cardObj =>
            {
                CardDragHandler drag = cardObj?.GetComponent<CardDragHandler>();
                return cardObj != null && (drag == null || !drag.IsDragging);
            });

        List<Vector2> positions = CalculateCardPositions(cardsToSort);
        for (int i = 0; i < cardsToSort.Count; i++)
        {
            GameObject obj = cardsToSort[i];
            if (obj == null) continue;

            RectTransform rt2 = obj.GetComponent<RectTransform>();
            if (rt2 != null)
            {
                LeanTween.cancel(rt2.gameObject);
                LeanTween.move(rt2, positions[i], 0.15f).setEaseOutQuad();

                // 오른쪽에 있는 카드일수록 위에 오도록 순서 부여
                rt2.SetSiblingIndex(i);
            }
        }

        if (handPanel != null)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(handPanel);
        }
    }

    // 일정 시간마다 자동으로 카드 드로우
    public IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            DrawCard(1);
        }
    }

    // 특정 카드를 손패 끝으로 이동 (선택 로직에 사용 가능)
    public void MoveCardToEnd(GameObject card)
    {
        if (handCards.Contains(card))
        {
            handCards.Remove(card);
            handCards.Add(card);
        }
    }

    void Update()
    {
        // 디버그용: R 키로 카드 위치 랜덤 섞기
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var card in handCards)
            {
                if (card != null)
                    card.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-10f, 10f), 0);
            }
        }
    }
}