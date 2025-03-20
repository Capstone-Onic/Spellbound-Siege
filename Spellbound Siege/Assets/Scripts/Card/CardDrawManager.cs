using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();  // �� ����Ʈ
    public Transform handTransform;  // UI���� ī�带 ǥ���� ��ġ
    public GameObject cardUIPrefab;  // ī�� UI ������

    private List<Card> drawPile = new List<Card>(); // ���� ���� ī�� ���

    void Start()
    {
        ResetDeck(); // ���� ���� �� ���� �ʱ�ȭ
        Debug.Log($"�� ����: {drawPile.Count}");
        DrawCard(5); // ó���� 5�� �̱�
        StartCoroutine(AutoDraw()); // �ڵ� ��ο� ����
    }

    public void ResetDeck()
    {
        drawPile = new List<Card>(deck); // �� ����
        Debug.Log($"�ʱ�ȭ �� �� ����: {drawPile.Count}");
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
        Debug.Log($"DrawCard ȣ��! ��û�� ī�� ��: {count}, ���� �� ����: {drawPile.Count}");

        for (int i = 0; i < count; i++)
        {
            if (drawPile.Count == 0)
            {
                Debug.LogWarning("���� ������ϴ�! �� �̻� ī�带 ���� �� ����.");
                return;
            }

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            Debug.Log($"ī�� {i + 1}��° �̱�: {drawnCard.cardName}, ���� �� ����: {drawPile.Count}");
            CreateCardUI(drawnCard);
        }
    }


    private void CreateCardUI(Card card)
    {
        GameObject cardObject = Instantiate(cardUIPrefab, handTransform);
        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.SetCard(card, this);

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            int cardIndex = handTransform.childCount - 1;
            rectTransform.anchoredPosition = new Vector2(150 * cardIndex, 0); // ī�� ���� ����
        }
    }

    public void RemoveCardFromHand(GameObject cardObject)
    {
        Destroy(cardObject); // ī�� ����
    }

    private IEnumerator AutoDraw()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10�� ���
            DrawCard(1); // ī�� 1�� ��ο�
        }
    }
}