using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;
using UnityEngine.UI;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public RectTransform handPanel;
    public GameObject cardPrefab;
    public int maxHandSize = 8;
    public RectTransform deckTransform;
    public float drawDuration = 0.5f;

    private List<Card> drawPile = new List<Card>();
    private List<GameObject> handCards = new List<GameObject>();
    private List<Card> usedCards = new();

    void Start()
    {
        if (handPanel != null)
            handPanel.gameObject.SetActive(false);
    }

    public void ResetDeck()
    {
        drawPile = new List<Card>(deck);
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
            if (drawPile.Count == 0)
            {
                if (usedCards.Count == DeckData.selectedDeck.Count && usedCards.Count > 0)
                {
                    drawPile = new List<Card>(usedCards);
                    usedCards.Clear();
                    ShuffleDeck();
                    Debug.Log("[CardDrawManager] 덱을 재충전하고 셔플함");
                }
                else
                {
                    Debug.Log("[CardDrawManager] 아직 모든 카드를 다 쓰지 않음 또는 카드가 없음 → 드로우 차단");
                    return;
                }
            }

            if (drawPile.Count == 0)
            {
                Debug.LogWarning("[CardDrawManager] drawPile이 비어있어서 드로우 불가");
                return;
            }

            if (handCards.Count >= maxHandSize) return;

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            StartCoroutine(CreateCardUI(drawnCard));
        }
    }

    private IEnumerator CreateCardUI(Card card)
    {
        handCards.RemoveAll(cardObj => cardObj == null);

        if (deckTransform == null || handPanel == null)
            yield break;

        GameObject cardObject = Instantiate(cardPrefab, handPanel);
        RectTransform rt = cardObject.GetComponent<RectTransform>();
        CardDisplay cd = cardObject.GetComponent<CardDisplay>();
        cd.SetCard(card);

        CanvasGroup cg = cardObject.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = cardObject.AddComponent<CanvasGroup>();

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

        Vector2 startPos = deckTransform.anchoredPosition;
        rt.anchoredPosition = startPos;

        handCards.Add(cardObject);

        List<GameObject> activeCards = new List<GameObject>(handCards);
        List<Vector2> positions = CalculateCardPositions(activeCards);

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

        for (int i = 0; i < activeCards.Count && i < positions.Count; i++)
        {
            GameObject obj = activeCards[i];
            if (obj == null) continue;
            RectTransform cardRt = obj.GetComponent<RectTransform>();
            if (cardRt == null) continue;
            cardRt.anchoredPosition = positions[i];
        }
    }

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

    public void RemoveCardFromHand(GameObject cardObject)
    {
        if (handCards.Contains(cardObject))
        {
            handCards.Remove(cardObject);
            Destroy(cardObject);
        }
        ReorganizeHand();
    }

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
                rt2.SetSiblingIndex(i);
            }
        }

        if (handPanel != null)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(handPanel);
        }
    }

    public IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            DrawCard(1);
        }
    }

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var card in handCards)
            {
                if (card != null)
                    card.GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-10f, 10f), 0);
            }
        }
    }

    public void AddUsedCard(Card card)
    {
        if (!usedCards.Contains(card))
        {
            usedCards.Add(card);
        }
    }
}