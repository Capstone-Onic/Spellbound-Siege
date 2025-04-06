using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spellbound;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ī���� �����͸� ��� ����
    public Card cardData;

    // ī�� UI ��ҵ�
    public Image cardImage;             // ī�� ��ü ��� Ȥ�� ���� �̹���
    public TMP_Text nameText;           // ī�� �̸� �ؽ�Ʈ
    public TMP_Text costText;           // ī�� �ڽ�Ʈ �ؽ�Ʈ
    public Image[] typeImages;          // ī�� Ÿ���� ��Ÿ���� �̹�����
    private Vector3 originalScale;
    private int originalSiblingIndex;
    Vector3 originalPosition;

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

    void Awake()
    {
        originalScale = transform.localScale;
    }

    // �ܺο��� ī�� �����͸� �޾ƿ� �����ϴ� �Լ�
    public void SetCard(Card card)
    {
        cardData = card;               // ī�� �����͸� ����
        UpdateCardDisplay();           // ī�� UI�� �� �����ͷ� ����

        // ����� �α�: ī�� �̸��� ����� ������ ���
        Debug.Log($"[ī�� ����] {card.cardName} �� {cardColors[(int)card.cardType[0]]}");
    }

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

    public GameObject glowBorder;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardDragHandler dragHandler = GetComponent<CardDragHandler>();
        if (dragHandler != null && dragHandler.IsDragging) return;

        if (glowBorder != null)
            glowBorder.SetActive(true);

        originalSiblingIndex = transform.GetSiblingIndex();
        originalPosition = transform.localPosition;

        transform.SetAsLastSibling();

        // ���� Tween �ߴ�
        LeanTween.cancel(gameObject);

        // Ȯ�� + ��ġ �̵�
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f);
        transform.localPosition += new Vector3(0, 50f, 0);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowBorder != null)
            glowBorder.SetActive(false);

        transform.SetSiblingIndex(originalSiblingIndex);

        // ���� Tween �ߴ�
        LeanTween.cancel(gameObject);

        // ��� + ��ġ ����
        LeanTween.scale(gameObject, originalScale, 0.2f);
        transform.localPosition = originalPosition;
    }

    public void UseCard()
    {
        // ī�� ��� �� ó�� �α�
        Debug.Log($"[����] {cardData.cardName}");

        // ī�� ��ο� �Ŵ����� �˸� �� ����Ʈ���� ���� + ����
        CardDrawManager manager = FindObjectOfType<CardDrawManager>();
        if (manager != null)
        {
            manager.RemoveCardFromHand(gameObject); // handCards���� ���� + ����
        }
        else
        {
            Destroy(gameObject); // ���� ó��
        }
    }
}