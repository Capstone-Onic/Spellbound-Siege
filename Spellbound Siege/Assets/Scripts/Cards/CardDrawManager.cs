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
        if (deckTransform == null || handPanel == null) yield break;

        int finalCardCount = handCards.Count + 1; // �� ī�� ���� ��
        List<Vector2> targetPositions = CalculateCardPositions(finalCardCount); // ���� ��ġ ���

        GameObject cardObject = Instantiate(cardPrefab, handPanel); // ī�� ������Ʈ ����
        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        cardDisplay.SetCard(card); // ī�� ������ ����

        // ī�� ���� ��ġ: �� ��ġ
        Vector2 startPosition = deckTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;

        // ī�� ��ǥ ��ġ: ������ �ε��� ��ġ
        Vector2 targetPosition = targetPositions[finalCardCount - 1];
        handCards.Add(cardObject); // ���п� �߰�

        // �ִϸ��̼� ó�� (�� �� �� ��ġ�� �̵�)
        float elapsedTime = 0f;
        while (elapsedTime < drawDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // ī�� ��ü ���� ����
        ReorganizeHand();
    }

    // ī�� ���� �°� ���п��� ��ġ ���� ���
    private List<Vector2> CalculateCardPositions(int cardCount)
    {
        List<Vector2> positions = new List<Vector2>();

        float panelWidth = handPanel.rect.width;
        float cardY = 50f; // ī���� Y ������

        if (cardCount == 0) return positions;

        if (cardCount == 1)
        {
            // ī�尡 �� ���̸� �߾� ��ġ
            positions.Add(new Vector2(0f, cardY));
            return positions;
        }

        float totalWidth = panelWidth;
        float leftEdge = -totalWidth / 2f;
        float spacing = totalWidth / (cardCount - 1); // ī�� �� ����

        for (int i = 0; i < cardCount; i++)
        {
            float x = leftEdge + i * spacing;
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