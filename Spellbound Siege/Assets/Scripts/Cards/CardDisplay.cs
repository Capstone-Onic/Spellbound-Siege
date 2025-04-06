using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spellbound;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 카드의 데이터를 담는 변수
    public Card cardData;

    // 카드 UI 요소들
    public Image cardImage;             // 카드 전체 배경 혹은 메인 이미지
    public TMP_Text nameText;           // 카드 이름 텍스트
    public TMP_Text costText;           // 카드 코스트 텍스트
    public Image[] typeImages;          // 카드 타입을 나타내는 이미지들
    private Vector3 originalScale;
    private int originalSiblingIndex;
    Vector3 originalPosition;

    // 카드 타입에 따른 배경 색상 배열 (주로 카드 전체 배경에 적용)
    private Color[] cardColors = {
        Color.red,    // fire (불)
        Color.blue,   // water (물)
        Color.cyan,   // ice (얼음)
        Color.gray,   // earth (땅)
        Color.black,  // dark (어둠)
        Color.yellow  // light (빛)
    };

    // 카드 타입에 따른 아이콘 색상 배열 (타입 아이콘에 적용)
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

    // 외부에서 카드 데이터를 받아와 설정하는 함수
    public void SetCard(Card card)
    {
        cardData = card;               // 카드 데이터를 저장
        UpdateCardDisplay();           // 카드 UI를 새 데이터로 갱신

        // 디버그 로그: 카드 이름과 적용된 색상을 출력
        Debug.Log($"[카드 색상] {card.cardName} → {cardColors[(int)card.cardType[0]]}");
    }

    // 카드 UI 요소를 카드 데이터에 맞춰 갱신하는 함수
    public void UpdateCardDisplay()
    {
        if (cardData == null)
            return; // 카드 데이터가 없으면 아무 것도 하지 않음

        // 카드의 첫 번째 타입에 따라 카드 이미지 색상을 설정
        if (cardData.cardType.Count > 0)
        {
            cardImage.color = cardColors[(int)cardData.cardType[0]];
        }

        // 카드 이름과 코스트 텍스트 설정
        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();

        // 카드 타입 아이콘들 설정
        for (int i = 0; i < typeImages.Length; i++)
        {
            if (i < cardData.cardType.Count)
            {
                // 해당 타입 인덱스에 맞는 색상으로 활성화
                typeImages[i].gameObject.SetActive(true);
                typeImages[i].color = typeColors[(int)cardData.cardType[i]];
            }
            else
            {
                // 카드 타입 수보다 많은 아이콘은 숨김
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

        // 기존 Tween 중단
        LeanTween.cancel(gameObject);

        // 확대 + 위치 이동
        LeanTween.scale(gameObject, originalScale * 1.2f, 0.2f);
        transform.localPosition += new Vector3(0, 50f, 0);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (glowBorder != null)
            glowBorder.SetActive(false);

        transform.SetSiblingIndex(originalSiblingIndex);

        // 기존 Tween 중단
        LeanTween.cancel(gameObject);

        // 축소 + 위치 복구
        LeanTween.scale(gameObject, originalScale, 0.2f);
        transform.localPosition = originalPosition;
    }

    public void UseCard()
    {
        // 카드 사용 시 처리 로그
        Debug.Log($"[사용됨] {cardData.cardName}");

        // 카드 드로우 매니저에 알림 → 리스트에서 제거 + 정렬
        CardDrawManager manager = FindObjectOfType<CardDrawManager>();
        if (manager != null)
        {
            manager.RemoveCardFromHand(gameObject); // handCards에서 제거 + 정렬
        }
        else
        {
            Destroy(gameObject); // 예외 처리
        }
    }
}