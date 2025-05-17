using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoldZoom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isHolding = false;
    private float holdTime = 2f;
    private float holdTimer = 0f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isZoomed = false;

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private CardDragHandler dragHandler;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        dragHandler = GetComponent<CardDragHandler>();
    }

    void Update()
    {
        if (isHolding && !isZoomed && !dragHandler.IsDragging)
        {
            holdTimer += Time.unscaledDeltaTime;

            if (holdTimer >= holdTime)
            {
                ZoomInCard();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isZoomed)
        {
            ZoomOutCard();
        }
        isHolding = false;
        holdTimer = 0f;
    }

    private void ZoomInCard()
    {
        isZoomed = true;
        Time.timeScale = 0.2f;

        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        // 부모를 캔버스로 바꾸되, 월드 좌표 유지하지 않음
        rectTransform.SetParent(parentCanvas.transform, false);
        rectTransform.SetAsLastSibling();

        // 화면 정중앙으로 이동
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;

        LeanTween.scale(gameObject, originalScale * 2f, 0.2f).setEaseOutQuad();
    }

    private void ZoomOutCard()
    {
        isZoomed = false;
        Time.timeScale = 1f;

        LeanTween.move(rectTransform, originalPosition, 0.2f).setEaseOutQuad();
        LeanTween.scale(gameObject, originalScale, 0.2f).setEaseOutQuad().setOnComplete(() =>
        {
            // 부모 복원 및 위치 재설정
            rectTransform.SetParent(dragHandler?.transform.parent, false);
            rectTransform.anchoredPosition = originalPosition;
        });
    }
}