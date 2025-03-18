using System.Collections.Generic;
using UnityEngine;

public class CardDrawManager : MonoBehaviour
{
    public List<Card> deck = new List<Card>();  // �� ����Ʈ
    public Transform handTransform;  // UI���� ī�带 ǥ���� ��ġ
    public GameObject cardUIPrefab;  // ī�� UI ������

    private List<Card> drawPile = new List<Card>(); // ���� ���� ī�� ���

    void Start()
    {
        ResetDeck(); // ���� ���� �� ���� �ʱ�ȭ
        DrawCard(5); // ó���� 5�� �̱�
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
            if (drawPile.Count == 0) return; // ���� ������� ����

            Card drawnCard = drawPile[0];
            drawPile.RemoveAt(0);
            CreateCardUI(drawnCard);
        }
    }

    private void CreateCardUI(Card card)
    {
        GameObject cardObject = Instantiate(cardUIPrefab, handTransform);
        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.SetCard(card);
    }
}