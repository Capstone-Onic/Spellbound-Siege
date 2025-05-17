using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Range Indicator")]
    public GameObject rangeIndicatorPrefab;
    public float indicatorOffsetHeight = 0.1f;

    [Header("Drag Settings")]
    public float dragScale = 1.2f;

    private GameObject rangeIndicatorInstance;
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

    private bool wasCardUsed = false;

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

        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalScale = transform.localScale;

        transform.SetParent(canvasRect, false);
        rectTransform.anchoredPosition = originalAnchoredPosition;
        transform.SetAsLastSibling();
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, initialScale * dragScale, 0.1f).setEaseOutQuad();
        canvasGroup.blocksRaycasts = false;

        if (rangeIndicatorPrefab != null && cardDisplay != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicatorPrefab);
            rangeIndicatorInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
            float r = cardDisplay.cardData.effectRadius;
            rangeIndicatorInstance.transform.localScale = new Vector3(r * 2, 0.01f, r * 2);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.localScale = initialScale * dragScale;

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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        wasCardUsed = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayerMask)
         && hit.collider.CompareTag("CardTarget"))
        {
            if (ManaManager.Instance.ConsumeMana(cardDisplay.cardData.cost))
            {
                Vector3 pos = hit.collider.transform.position;
                CardEffectProcessor.ApplyCardEffectToTile(cardDisplay.cardData, null, pos);
                wasCardUsed = true;
            }
            else
            {
                Debug.Log("마나 부족으로 카드 사용 불가");
                ManaManager.Instance.ShowManaWarning();
            }
        }

        if (rangeIndicatorInstance != null)
        {
            Destroy(rangeIndicatorInstance);
            rangeIndicatorInstance = null;
        }

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

        if (wasCardUsed)
            StartCoroutine(DelayedCardUse());
    }

    private IEnumerator DelayedCardUse()
    {
        yield return new WaitForSeconds(0.1f);
        cardDisplay.UseCard();
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