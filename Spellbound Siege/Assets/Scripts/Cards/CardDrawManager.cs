using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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

    // ���� �� ���� �ʱ�ȭ�ϰ�, �ʱ� ī�� 5�� ��ο� �� �ڵ� ��ο� ����
    void Start()
    {
        ResetDeck();
        DrawCard(5);
        StartCoroutine(AutoDraw());
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
            // ��ο��� ī�尡 ���ų� ���� ���� á�� ��� �ߴ�
            if (drawPile.Count == 0 || handCards.Count >= maxHandSize)
                return;

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            StartCoroutine(CreateCardUI(drawnCard)); // ī�� UI ���� �� �ִϸ��̼� ó��
        }
    }

    // ī�� UI�� �����ϰ� ������ �� ��ġ�� �̵��ϴ� �ڷ�ƾ
    private IEnumerator CreateCardUI(Card card)
    {
        // 1. �ı��� ī�� ����
        handCards.RemoveAll(cardObj => cardObj == null);

        if (deckTransform == null || handPanel == null) yield break;

        // 2. �� ī�� ����
        GameObject cardObject = Instantiate(cardPrefab, handPanel);
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        cardDisplay.SetCard(card);

        // 3. �� ��ġ���� ����
        Vector2 startPosition = deckTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;

        // 4. ����Ʈ�� �߰�
        handCards.Add(cardObject);

        // 5. �巡�� ���� �ƴ� ī�常 ���� ���
        List<GameObject> activeCards = handCards.FindAll(card =>
        {
            CardDragHandler drag = card?.GetComponent<CardDragHandler>();
            return card != null && (drag == null || !drag.IsDragging);
        });

        List<Vector2> positions = CalculateCardPositions(activeCards);

        // 6. �ִϸ��̼� �̵�
        float elapsedTime = 0f;
        while (elapsedTime < drawDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawDuration);

            int count = Mathf.Min(activeCards.Count, positions.Count);
            for (int i = 0; i < count; i++)
            {
                if (activeCards[i] == null) continue;

                RectTransform rt = activeCards[i].GetComponent<RectTransform>();
                if (rt == null) continue;

                Vector2 target = positions[i];
                rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, target, t);
            }

            yield return null;
        }

        // 7. ��ġ ����
        for (int i = 0; i < activeCards.Count; i++)
        {
            if (activeCards[i] == null) continue;

            RectTransform rt = activeCards[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = positions[i];
        }
    }

    // ī�� ���� �°� ���п��� ��ġ ���� ���
    private List<Vector2> CalculateCardPositions(List<GameObject> activeCards)
    {
        List<Vector2> positions = new List<Vector2>();

        int cardCount = activeCards.Count;
        if (cardCount == 0) return positions;

        float cardY = -25f;

        // ī�� �� �ִ� ���� ���� (���� ���̰� �����)
        float maxSpacing = 160f;
        float spacing = Mathf.Min(maxSpacing, handPanel.rect.width / cardCount);

        // �ʹ� ���� �ʰ� �ּ� ���ݵ� ���� (optional)
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
            Destroy(cardObject); // �� ���Ŵ� ��������
        }
        ReorganizeHand();
    }

    // �տ� �ִ� ī����� ��ġ ���ġ
    public void ReorganizeHand(bool includeAllCards = false)
    {
        List<GameObject> cardsToSort = includeAllCards
            ? handCards
            : handCards.FindAll(card =>
            {
                CardDragHandler drag = card?.GetComponent<CardDragHandler>();
                return card != null && (drag == null || !drag.IsDragging);
            });

        List<Vector2> positions = CalculateCardPositions(cardsToSort);

        for (int i = 0; i < cardsToSort.Count; i++)
        {
            if (cardsToSort[i] == null) continue;

            RectTransform rt = cardsToSort[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                LeanTween.cancel(rt.gameObject);
                LeanTween.move(rt, positions[i], 0.15f).setEaseOutQuad();
                rt.SetSiblingIndex(i);
            }
        }

        // ���̾ƿ� ���� ������
        if (handPanel != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(handPanel);
        }
    }

    // ���� �ð����� �ڵ����� ī�� ��ο�
    private IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5�ʸ���
            DrawCard(1);
        }
    }

    // CardDrawManager.cs �ȿ� �߰�
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
}