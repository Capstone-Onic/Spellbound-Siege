using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Spellbound;

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

        // 4. ī�� ����Ʈ�� �߰�
        handCards.Add(cardObject);

        // 5. ī�� ���� ���� ��ġ ���
        List<Vector2> positions = CalculateCardPositions(handCards.Count);

        // 6. �̵� �ִϸ��̼� ����
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

        // 7. ���� ������ (��ġ ����)
        int finalCount = Mathf.Min(handCards.Count, positions.Count);
        for (int i = 0; i < finalCount; i++)
        {
            if (handCards[i] == null) continue;

            RectTransform rt = handCards[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = positions[i];
        }
    }

    // ī�� ���� �°� ���п��� ��ġ ���� ���
    private List<Vector2> CalculateCardPositions(int cardCount)
    {
        List<Vector2> positions = new List<Vector2>();

        float cardY = -25f; // Y ��ġ ����

        if (cardCount == 0) return positions;

        // ī�� �� �ִ� ���� ���� (��ġ�� ����� ���� ����)
        float maxSpacing = 160f; // spacing�� ���̷��� �� �� ���̱�
        float spacing = Mathf.Min(maxSpacing, handPanel.rect.width / cardCount);

        // ��ü ī�� �ʺ�
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

    // ���� �ð����� �ڵ����� ī�� ��ο�
    private IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5�ʸ���
            DrawCard(1);
        }
    }
}