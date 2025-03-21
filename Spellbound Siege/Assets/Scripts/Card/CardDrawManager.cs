using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();  // 덱 리스트
    public RectTransform handPanel;  // HandPanel UI의 RectTransform
    public GameObject cardUIPrefab;  // 카드 UI 프리팹
    public int maxHandSize = 8; // 손에 들 수 있는 최대 카드 개수
    public RectTransform deckTransform; // 덱의 위치 (카드가 생성되는 시작점)
    public float drawDuration = 0.5f; // 카드 드로우 애니메이션 지속 시간

    private List<Card> drawPile = new List<Card>(); // 실제 뽑을 카드 목록
    private List<GameObject> handCards = new List<GameObject>(); // 손에 있는 카드 목록

    void Start()
    {
        ResetDeck();
        DrawCard(5); // 처음에 5장 뽑기
        StartCoroutine(AutoDraw()); // 자동 드로우 시작
    }

    public void ResetDeck()
    {
        drawPile = new List<Card>(deck); // 덱 복사
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

        // 카드 UI 생성 및 부모 설정 (handPanel의 자식으로 넣어야 좌표 이동이 정확함)
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

        // deckTransform의 위치를 정확히 받아옴
        Vector2 startPosition = deckTransform.anchoredPosition;
        rectTransform.anchoredPosition = startPosition;

        // 목표 위치 설정 (HandPanel 내의 자리)
        Vector2 targetPosition = new Vector2(150 * (handCards.Count - 1), 0);

        // 애니메이션 재생
        float elapsedTime = 0f;
        while (elapsedTime < drawDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / drawDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // 최종 위치로 고정
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
