using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // RawImage 사용

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Range Indicator")]
    [Tooltip("드래그 중 범위 표시용 프리팹 (투명 원형)")]
    public GameObject rangeIndicatorPrefab;
    [Tooltip("콜라이더 최상단에서 띄울 오프셋 높이")]
    public float indicatorOffsetHeight = 0.1f;

    [Header("Drag Settings")]
    [Tooltip("드래그 중 카드 크기 배율")]
    public float dragScale = 1.2f;

    [Header("Option Line (Texture2D)")]
    [Tooltip("드래그 중 카드에서 마우스까지 연결선을 표시할 Texture2D")]
    public Texture2D optionLineTexture;

    // Runtime instances
    private GameObject rangeIndicatorInstance;
    private RectTransform optionLineRect;
    private RawImage optionLineImage;
    private float optionLineHeight;

    // Core components & state
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;
    private CardDisplay cardDisplay;

    private Transform originalParent;
    private Vector2 originalAnchoredPosition;
    private Vector3 initialScale;
    private Vector3 originalScale;
    private bool isDragging;
    public bool IsDragging => isDragging;

    private TileHighlighter lastHighlightedTile;
    private int raycastLayerMask;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        cardDisplay = GetComponent<CardDisplay>();
        initialScale = transform.localScale;
        raycastLayerMask = ~LayerMask.GetMask("Ignore Raycast");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // 상태 저장
        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalScale = transform.localScale;

        // Canvas 최상위로, 위치 유지한 채로 확대
        transform.SetParent(canvasRect, false);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        transform.SetAsLastSibling();
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, initialScale * dragScale, 0.1f).setEaseOutQuad();
        canvasGroup.blocksRaycasts = false;

        // Range Indicator 생성
        if (rangeIndicatorPrefab != null && cardDisplay != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab);
            rangeIndicatorInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
            float r = cardDisplay.cardData.effectRadius;
            rangeIndicatorInstance.transform.localScale = new Vector3(r * 2, 0.01f, r * 2);
        }

        // Option Line 생성
        if (optionLineTexture != null)
        {
            GameObject go = new GameObject("OptionLine", typeof(RawImage));
            go.transform.SetParent(canvasRect, false);

            optionLineRect = go.GetComponent<RectTransform>();
            optionLineImage = go.GetComponent<RawImage>();
            optionLineImage.texture = optionLineTexture;

            optionLineRect.pivot = new Vector2(0f, 0.5f);
            optionLineHeight = optionLineRect.sizeDelta.y;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // **카드가 항상 커진 상태를 유지하도록 강제**
        transform.localScale = initialScale * dragScale;

        // --- Range Indicator 업데이트 ---
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayerMask))
        {
            if (rangeIndicatorInstance != null)
            {
                float topY = hit.collider.bounds.max.y;
                Vector3 pos = hit.point;
                pos.y = topY + indicatorOffsetHeight;
                rangeIndicatorInstance.transform.position = pos;
            }

            var tile = hit.collider.GetComponent<TileHighlighter>();
            if (tile != null)
            {
                if (lastHighlightedTile != null && lastHighlightedTile != tile)
                    lastHighlightedTile.SetHighlighted(false);
                tile.SetHighlighted(true);
                lastHighlightedTile = tile;
            }
            else if (lastHighlightedTile != null)
            {
                lastHighlightedTile.SetHighlighted(false);
                lastHighlightedTile = null;
            }
        }

        // --- Option Line 업데이트 ---
        if (optionLineRect != null)
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, canvas.worldCamera, out localMousePos);

            Vector2 cardLocalPos = originalAnchoredPosition;
            Vector2 dir = localMousePos - cardLocalPos;
            float dist = dir.magnitude;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            optionLineRect.anchoredPosition = cardLocalPos;
            optionLineRect.localRotation = Quaternion.Euler(0f, 0f, ang);
            optionLineRect.sizeDelta = new Vector2(dist, optionLineHeight);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        // 카드 사용 처리
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayerMask)
         && hit.collider.CompareTag("CardTarget"))
        {
            Vector3 pos = hit.collider.transform.position;
            CardEffectProcessor.ApplyCardEffectToTile(cardDisplay.cardData, null, pos);
            cardDisplay.UseCard();
        }

        // Range Indicator 제거
        if (rangeIndicatorInstance != null)
        {
            Destroy(rangeIndicatorInstance);
            rangeIndicatorInstance = null;
        }

        // Option Line 제거
        if (optionLineRect != null)
            Destroy(optionLineRect.gameObject);

        // 원래 위치·부모·크기로 복귀
        StartCoroutine(ReturnToHand());
    }

    private IEnumerator ReturnToHand()
    {
        transform.SetParent(originalParent, false);
        yield return null;

        rectTransform.anchoredPosition = originalAnchoredPosition;
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, initialScale, 0.15f).setEaseOutQuad();
        yield return StartCoroutine(ShakeCard());
        FindObjectOfType<CardDrawManager>()?.ReorganizeHand(includeAllCards: true);
    }

    private IEnumerator ShakeCard()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float shakeAmt = 10f;
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Mathf.Sin(elapsed * 60f) * shakeAmt * (1 - elapsed / duration);
            rectTransform.anchoredPosition = startPos + new Vector2(offsetX, 0);
            yield return null;
        }

        rectTransform.anchoredPosition = startPos;
    }
}
