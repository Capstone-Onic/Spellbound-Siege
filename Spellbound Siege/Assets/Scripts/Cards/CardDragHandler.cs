using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Transform originalParent;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        transform.SetParent(originalParent.root); // 최상위로 이동하여 UI 겹침 방지
        canvasGroup.alpha = 0.6f; // 반투명 효과
        canvasGroup.blocksRaycasts = false; // 다른 UI 감지 가능하게 설정
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition; // 마우스 위치로 이동
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 특정 영역(예: 필드)에 놓인 경우 카드 사용
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DropZone"))
        {
            UseCard();
        }
        else
        {
            // 원래 위치로 돌아감
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void UseCard()
    {
        Debug.Log($"카드 사용됨: {gameObject.name}");
        Destroy(gameObject); // 카드 사용 후 제거
    }
}