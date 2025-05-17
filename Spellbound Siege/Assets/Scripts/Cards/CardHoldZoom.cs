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

        // �θ� ĵ������ �ٲٵ�, ���� ��ǥ �������� ����
        rectTransform.SetParent(parentCanvas.transform, false);
        rectTransform.SetAsLastSibling();

        // ȭ�� ���߾����� �̵�
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
            // �θ� ���� �� ��ġ �缳��
            rectTransform.SetParent(dragHandler?.transform.parent, false);
            rectTransform.anchoredPosition = originalPosition;
        });
    }
}