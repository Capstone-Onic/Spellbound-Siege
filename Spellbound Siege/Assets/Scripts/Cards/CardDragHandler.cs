using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector2 originalAnchoredPosition;
    private int originalSiblingIndex;
    private Vector3 originalScale;
    private Vector3 initialScale;
    private bool isDragging = false;
    public bool IsDragging => isDragging; // 읽기 전용 프로퍼티

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialScale = transform.localScale; // 항상 기준되는 크기
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalScale = transform.localScale;

        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;

        LeanTween.cancel(gameObject);

        // 드래그 시 살짝 작아짐
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, initialScale * 0.5f, 0.1f).setEaseOutQuad();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.lossyScale;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        bool success = eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DropZone");

        if (success)
        {
            GetComponent<CardDisplay>().UseCard();
        }
        else
        {
            StartCoroutine(ReturnToHand());
        }

        LeanTween.scale(gameObject, originalScale, 0.15f).setEaseOutQuad();
    }

    private IEnumerator ReturnToHand()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        yield return null; // UI 시스템이 부모 적용되도록 대기

        rectTransform.anchoredPosition = originalAnchoredPosition;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, 0.15f).setEaseOutQuad();
        StartCoroutine(ShakeCard());
    }

    private IEnumerator ShakeCard()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float shakeAmount = 10f;
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Mathf.Sin(elapsed * 60f) * shakeAmount * (1 - elapsed / duration);
            rectTransform.anchoredPosition = startPos + new Vector2(offsetX, 0);
            yield return null;
        }

        rectTransform.anchoredPosition = startPos;
    }
}