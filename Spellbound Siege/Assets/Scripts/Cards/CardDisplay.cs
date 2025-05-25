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
    public Image cardImage;             // 카드 전체 배경
    public TMP_Text nameText;
    public TMP_Text costText;
    public Image[] typeImages;
    public Image artworkImage;          // ImageFrame > Image 에 해당

    [Header("Hover Effect")]
    public GameObject glowBorder;
    private Vector3 originalScale;
    private int originalSiblingIndex;
    private Vector3 originalPosition;

    public TextMeshProUGUI infoText;  // 카드 설명 텍스트 UI

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
        if (originalScale == Vector3.zero) // 이미 설정된 경우 재설정 방지
            originalScale = transform.localScale;
    }

    void Start()
    {
        if (cardData != null)
            UpdateCardDisplay();

        // GameScene이 아니면 확대/클릭 방해 요소 해제
        if (SceneManager.GetActiveScene().name != "GameScene")
        {
            var cg = GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }

            // 선택 마크나 Glow 같은 효과 비활성화해도 좋음
            if (glowBorder != null)
                glowBorder.SetActive(false);
        }
    }

    public void SetCard(Card card)
    {
        cardData = card;
        UpdateCardDisplay();

        Debug.Log($"[카드 색상] {card.cardName} → {cardColors[(int)card.cardType[0]]}");
    }

    public void UpdateCardDisplay()
    {
        if (cardData == null) return;

        // 카드 배경 색상
        if (cardData.cardType.Count > 0)
            cardImage.color = cardColors[(int)cardData.cardType[0]];

        // 이름과 코스트
        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();

        // 속성 아이콘 색상
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

        // 카드 일러스트 이미지 설정
        if (artworkImage != null && cardData.cardImage != null)
        {
            artworkImage.sprite = cardData.cardImage;
        }
        else
        {
            Debug.LogWarning($"[카드이미지없음] {cardData.cardName}의 이미지가 설정되지 않았습니다.");
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

        // 드래그 중이거나 확대 상태면 Hover 효과 비활성화
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
        // 게임 씬이 아니면 비활성화
        if (SceneManager.GetActiveScene().name != "GameScene") return;

        Debug.Log($"[사용됨] {cardData.cardName}");

        CardDrawManager manager = FindObjectOfType<CardDrawManager>();
        if (manager != null)
        {
            manager.RemoveCardFromHand(gameObject);

            // 카드 사용 기록 추가
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