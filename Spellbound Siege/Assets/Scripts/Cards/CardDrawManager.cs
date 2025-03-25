using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();  // �� ����Ʈ
    public RectTransform handPanel;  // HandPanel UI�� RectTransform
    public GameObject cardUIPrefab;  // ī�� UI ������
    public int maxHandSize = 8; // �տ� �� �� �ִ� �ִ� ī�� ����
    public RectTransform deckTransform; // ���� ��ġ (ī�尡 �����Ǵ� ������)
    public float drawDuration = 0.5f; // ī�� ��ο� �ִϸ��̼� ���� �ð�

    private List<Card> drawPile = new List<Card>(); // ���� ���� ī�� ���
    private List<GameObject> handCards = new List<GameObject>(); // �տ� �ִ� ī�� ���

    void Start()
    {
        ResetDeck();
        DrawCard(5); // ó���� 5�� �̱�
        StartCoroutine(AutoDraw()); // �ڵ� ��ο� ����
    }

    public void ResetDeck()
    {
        drawPile = new List<Card>(deck); // �� ����
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
            if (drawPile.Count == 0 || handCards.Count >= maxHandSize)
                return;

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            StartCoroutine(CreateCardUI(drawnCard));
        }
    }

    private IEnumerator CreateCardUI(Card card)
    {
        if (deckTransform == null)
        {
            Debug.LogError("Deck Transform is not assigned in the Inspector!");
            yield break;
        }

        if (handPanel == null)
        {
            Debug.LogError("Hand Panel is not assigned in the Inspector!");
            yield break;
        }

        // ī�� UI ���� �� �θ� ���� (handPanel�� �ڽ����� �־�� ��ǥ �̵��� ��Ȯ��)
        GameObject cardObject = Instantiate(cardUIPrefab, handPanel);
        if (cardObject == null)
        {
            Debug.LogError("Failed to instantiate cardUIPrefab!");
            yield break;
        }

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("Card Prefab is missing a RectTransform component!");
            yield break;
        }

        CardUI cardUI = cardObject.GetComponent<CardUI>();
        if (cardUI == null)
        {
            Debug.LogError("CardUI script is missing from the card prefab!");
            yield break;
        }

        cardUI.SetCard(card, this);
        handCards.Add(cardObject);

        // deckTransform�� ��ġ�� ��Ȯ�� �޾ƿ�
        Vector2 startPosition = deckTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;

        // ��ǥ ��ġ ���� (HandPanel ���� �ڸ�)
        Vector2 targetPosition = new Vector2(150 * (handCards.Count - 1), 0);

        // �ִϸ��̼� ���
        float elapsedTime = 0f;
        while (elapsedTime < drawDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // ���� ��ġ�� ����
        rectTransform.anchoredPosition = targetPosition;
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

    private void ReorganizeHand()
    {
        for (int i = 0; i < handCards.Count; i++)
        {
            RectTransform rectTransform = handCards[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(150 * i, 0);
            }
        }
    }

    private IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            DrawCard(1);
        }
    }
}
