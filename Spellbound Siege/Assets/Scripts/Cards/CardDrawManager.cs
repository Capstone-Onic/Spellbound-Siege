using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;
using UnityEngine.UI;

public class CardDrawManager : MonoBehaviour
{
    // ī�� �� (�ʱ� ī�� ����Ʈ)
    public List<Card> deck = new List<Card>();

    // ī�尡 ��ġ�� �� �г� (UI ���� ��ġ)
    public RectTransform handPanel;

    // ī�� ������ (UI ī�� ������Ʈ)
    public GameObject cardPrefab;

    // �ִ� ���� ũ��
    public int maxHandSize = 8;

    // ī�尡 ���� ��ġ(�� ��ġ)
    public RectTransform deckTransform;

    // ī�� ��ο� �ִϸ��̼� �ð�
    public float drawDuration = 0.5f;

    // ���� ��ο쿡 ����� ī�� ����
    private List<Card> drawPile = new List<Card>();

    // ���� �տ� �ִ� ī�� ������Ʈ��
    private List<GameObject> handCards = new List<GameObject>();

    void Start()
    {
        if (handPanel != null)
            handPanel.gameObject.SetActive(false);
    }

    // �� �ʱ�ȭ (��ο� ���� ī�� ���� + ����)
    public void ResetDeck()
    {
        drawPile = new List<Card>(deck);
        ShuffleDeck();
    }

    // �� ����
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

    // ������ ����ŭ ī�带 ��ο�
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

    // ī�� UI�� �����ϰ� ������ �� ��ġ�� �̵��ϴ� �ڷ�ƾ
    private IEnumerator CreateCardUI(Card card)
    {
        // ���п��� �ı��� ī�� ����
        handCards.RemoveAll(cardObj => cardObj == null);

        if (deckTransform == null || handPanel == null)
            yield break;

        // �� ī�� ����
        GameObject cardObject = Instantiate(cardPrefab, handPanel);
        RectTransform rt = cardObject.GetComponent<RectTransform>();
        CardDisplay cd = cardObject.GetComponent<CardDisplay>();
        cd.SetCard(card);

        // CanvasGroup�� ������ �߰�
        CanvasGroup cg = cardObject.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = cardObject.AddComponent<CanvasGroup>();

        // Ȯ�� ���� ī�尡 �ϳ��� �ִٸ� ���콺 ���� ��Ȱ��ȭ
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

        // �� ��ġ���� ���� ��ġ ����
        Vector2 startPos = deckTransform.anchoredPosition;
        rt.anchoredPosition = startPos;

        // ����Ʈ�� �߰�
        handCards.Add(cardObject);

        // ��� ī�� ��ġ ���
        List<GameObject> activeCards = new List<GameObject>(handCards);
        List<Vector2> positions = CalculateCardPositions(activeCards);

        // �ִϸ��̼� �̵�
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

        // ���� ��ġ ����
        for (int i = 0; i < activeCards.Count && i < positions.Count; i++)
        {
            GameObject obj = activeCards[i];
            if (obj == null) continue;
            RectTransform cardRt = obj.GetComponent<RectTransform>();
            if (cardRt == null) continue;
            cardRt.anchoredPosition = positions[i];
        }
    }

    // ī�� ���� �°� ���п��� ��ġ ���� ���
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

    // �տ��� ī�� ���� �� ���� ����
    public void RemoveCardFromHand(GameObject cardObject)
    {
        if (handCards.Contains(cardObject))
        {
            handCards.Remove(cardObject);
            Destroy(cardObject);
        }
        ReorganizeHand();
    }

    // �տ� �ִ� ī����� ��ġ ���ġ
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

                // �����ʿ� �ִ� ī���ϼ��� ���� ������ ���� �ο�
                rt2.SetSiblingIndex(i);
            }
        }

        if (handPanel != null)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(handPanel);
        }
    }

    // ���� �ð����� �ڵ����� ī�� ��ο�
    public IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            DrawCard(1);
        }
    }

    // Ư�� ī�带 ���� ������ �̵� (���� ������ ��� ����)
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
        // ����׿�: R Ű�� ī�� ��ġ ���� ����
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