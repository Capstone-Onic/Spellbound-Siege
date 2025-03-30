using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spellbound;

public class CardDisplay : MonoBehaviour
{
    // ī���� �����͸� ��� ����
    public Card cardData;

    // ī�� UI ��ҵ�
    public Image cardImage;             // ī�� ��ü ��� Ȥ�� ���� �̹���
    public TMP_Text nameText;           // ī�� �̸� �ؽ�Ʈ
    public TMP_Text costText;           // ī�� �ڽ�Ʈ �ؽ�Ʈ
    public Image[] typeImages;          // ī�� Ÿ���� ��Ÿ���� �̹�����

    // ī�� Ÿ�Կ� ���� ��� ���� �迭 (�ַ� ī�� ��ü ��濡 ����)
    private Color[] cardColors = {
        Color.red,    // fire (��)
        Color.blue,   // water (��)
        Color.cyan,   // ice (����)
        Color.gray,   // earth (��)
        Color.black,  // dark (���)
        Color.yellow  // light (��)
    };

    // ī�� Ÿ�Կ� ���� ������ ���� �迭 (Ÿ�� �����ܿ� ����)
    private Color[] typeColors = {
        Color.red,    // fire
        Color.blue,   // water
        Color.cyan,   // ice
        Color.gray,   // earth
        Color.black,  // dark
        Color.yellow  // light
    };

    // �ܺο��� ī�� �����͸� �޾ƿ� �����ϴ� �Լ�
    public void SetCard(Card card)
    {
        cardData = card;               // ī�� �����͸� ����
        UpdateCardDisplay();           // ī�� UI�� �� �����ͷ� ����

        // ����� �α�: ī�� �̸��� ����� ������ ���
        Debug.Log($"[ī�� ����] {card.cardName} �� {cardColors[(int)card.cardType[0]]}");
    }

    /*
    // �ڵ� ������ �ʿ��� ��� Start���� ȣ�� ����
    void Start()
    {
        UpdateCardDisplay();
    }
    */

    // ī�� UI ��Ҹ� ī�� �����Ϳ� ���� �����ϴ� �Լ�
    public void UpdateCardDisplay()
    {
        if (cardData == null)
            return; // ī�� �����Ͱ� ������ �ƹ� �͵� ���� ����

        // ī���� ù ��° Ÿ�Կ� ���� ī�� �̹��� ������ ����
        if (cardData.cardType.Count > 0)
        {
            cardImage.color = cardColors[(int)cardData.cardType[0]];
        }

        // ī�� �̸��� �ڽ�Ʈ �ؽ�Ʈ ����
        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();

        // ī�� Ÿ�� �����ܵ� ����
        for (int i = 0; i < typeImages.Length; i++)
        {
            if (i < cardData.cardType.Count)
            {
                // �ش� Ÿ�� �ε����� �´� �������� Ȱ��ȭ
                typeImages[i].gameObject.SetActive(true);
                typeImages[i].color = typeColors[(int)cardData.cardType[i]];
            }
            else
            {
                // ī�� Ÿ�� ������ ���� �������� ����
                typeImages[i].gameObject.SetActive(false);
            }
        }
    }
}