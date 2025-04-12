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
    public bool IsDragging => isDragging;
    private TileHighlighter lastHighlightedTile;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        originalAnchoredPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        originalScale = transform.localScale;

        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, initialScale * 0.5f, 0.1f).setEaseOutQuad();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / transform.lossyScale;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileHighlighter tile = hit.collider.GetComponent<TileHighlighter>();

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

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 타겟이 "CardTarget" 태그를 가졌을 때만 카드 효과 적용
            if (hit.collider.CompareTag("CardTarget"))
            {
                Debug.Log("카드 사용: rock 타일에 드롭됨");

                // 카드 효과 → 해당 위치에서 enemy 탐색
                Vector3 position = hit.collider.transform.position;
                Collider[] hits = Physics.OverlapSphere(position, 0.4f);
                foreach (var h in hits)
                {
                    var tile = h.GetComponent<Collider>();
                    if (tile != null)
                    {
                        CardEffectProcessor.ApplyCardEffectToTile(GetComponent<CardDisplay>().cardData, null, position);
                        GetComponent<CardDisplay>().UseCard();
                        return;
                    }
                }
            }
        }

        StartCoroutine(ReturnToHand());
        LeanTween.scale(gameObject, originalScale, 0.15f).setEaseOutQuad();

        if (lastHighlightedTile != null)
        {
            lastHighlightedTile.SetHighlighted(false);
            lastHighlightedTile = null;
        }
    }

    private IEnumerator ReturnToHand()
    {
        transform.SetParent(originalParent);
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