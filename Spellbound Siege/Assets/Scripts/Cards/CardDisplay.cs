using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spellbound;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Card cardData;

    [Header("UI Elements")]
    public Image cardImage;             // ī�� ��ü ���
    public TMP_Text nameText;
    public TMP_Text costText;
    public Image[] typeImages;
    public Image artworkImage;          // ImageFrame > Image �� �ش�

    [Header("Hover Effect")]
    public GameObject glowBorder;
    private Vector3 originalScale;
    private int originalSiblingIndex;
    private Vector3 originalPosition;

    public TextMeshProUGUI infoText;  // ī�� ���� �ؽ�Ʈ UI

    private Color[] cardColors = {
        Color.red, 
        Color.blue, 
        Color.cyan, 
        Color.gray, 
        Color.black, 
        Color.yellow
    };

    private Color[] typeColors = {
        Color.red, 
        Color.blue, 
        Color.cyan, 
        Color.gray, 
        Color.black, 
        Color.yellow
    };

    void Awake()
    {
        if (originalScale == Vector3.zero) // �̹� ������ ��� �缳�� ����
            originalScale = transform.localScale;
    }

    void Start()
    {
        if (cardData != null)
            UpdateCardDisplay();

        // GameScene�� �ƴϸ� Ȯ��/Ŭ�� ���� ��� ����
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            var cg = GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }

            // ���� ��ũ�� Glow ���� ȿ�� ��Ȱ��ȭ�ص� ����
            if (glowBorder != null)
                glowBorder.SetActive(false);
        }
    }

    public void SetCard(Card card)
    {
        cardData = card;
        UpdateCardDisplay();

        Debug.Log($"[ī�� ����] {card.cardName} �� {cardColors[(int)card.cardType[0]]}");
    }

    public void UpdateCardDisplay()
    {
        if (cardData == null) return;

        // ī�� ��� ����
        if (cardData.cardType.Count > 0)
            cardImage.color = cardColors[(int)cardData.cardType[0]];

        // �̸��� �ڽ�Ʈ
        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();

        // �Ӽ� ������ ����
        for (int i = 0; i < typeImages.Length; i++)
        {
            if (i < cardData.cardType.Count)
            {
                typeImages[i].gameObject.SetActive(true);
                typeImages[i].color = typeColors[(int)cardData.cardType[i]];
            }
            else
            {
                typeImages[i].gameObject.SetActive(false);
            }
        }

        // ī�� �Ϸ���Ʈ �̹��� ����
        if (artworkImage != null && cardData.cardImage != null)
        {
            artworkImage.sprite = cardData.cardImage;
        }
        else
        {
            Debug.LogWarning($"[ī���̹�������] {cardData.cardName}�� �̹����� �������� �ʾҽ��ϴ�.");
        }

        if (infoText != null)
        {
            infoText.text = cardData.description;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        if (IsCardInDeckOrRewardUI()) return;

        CardDragHandler dragHandler = GetComponent<CardDragHandler>();
        CardHoldZoom holdZoom = GetComponent<CardHoldZoom>();

        // �巡�� ���̰ų� Ȯ�� ���¸� Hover ȿ�� ��Ȱ��ȭ
        if ((dragHandler != null && dragHandler.IsDragging) || (holdZoom != null && holdZoom.IsZooming)) return;

        if (glowBorder != null)
            glowBorder.SetActive(true);

        originalSiblingIndex = transform.GetSiblingIndex();
        originalPosition = transform.localPosition;

        transform.SetAsLastSibling();
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f);
        transform.localPosition += new Vector3(0, 50f, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        if (glowBorder != null)
            glowBorder.SetActive(false);

        transform.SetSiblingIndex(originalSiblingIndex);
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, originalScale, 0.2f);
        transform.localPosition = originalPosition;
    }

    public void UseCard()
    {
        // ���� ���� �ƴϸ� ��Ȱ��ȭ
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        Debug.Log($"[����] {cardData.cardName}");

        CardDrawManager manager = FindObjectOfType<CardDrawManager>();
        if (manager != null)
        {
            manager.RemoveCardFromHand(gameObject);

            // ī�� ��� ��� �߰�
            manager.AddUsedCard(cardData);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool IsCardInDeckOrRewardUI()
    {
        var deckPanel = GameObject.Find("DeckSettingPanel");
        var rewardPanel = GameObject.Find("CardSelectPanel");

        return (deckPanel != null && transform.IsChildOf(deckPanel.transform)) ||
               (rewardPanel != null && transform.IsChildOf(rewardPanel.transform));
    }
}