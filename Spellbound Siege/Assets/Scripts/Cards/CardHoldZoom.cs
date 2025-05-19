using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHoldZoom : MonoBehaviour, IPointerDownHandler
{
    private bool isHolding = false;
    private bool isZoomed = false;
    public bool IsZooming => isZoomed;
    private float holdTime = 1f;
    private float holdTimer = 0f;

    private Canvas parentCanvas;
    private CardDragHandler dragHandler;
    private CardDisplay originalCardDisplay;
    private CardDisplay cardDisplay;

    private GameObject zoomedCardInstance;
    private RectTransform zoomedCardRect;
    private Vector3 originalScale;

    [Header("Zoom Visual Effect")]
    public GameObject screenEdgeEffectPrefab; // �׵θ� ����Ʈ ������
    private GameObject currentEffectInstance;

    void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();

        // ��ü Canvas ã��
        GameObject rootCanvasGO = GameObject.Find("Canvas");
        if (rootCanvasGO != null)
            parentCanvas = rootCanvasGO.GetComponent<Canvas>();
        else
            Debug.LogError("[CardHoldZoom] Canvas ������Ʈ�� ã�� �� �����ϴ�!");

        dragHandler = GetComponent<CardDragHandler>();
        originalCardDisplay = GetComponent<CardDisplay>();
        originalScale = Vector3.one;
    }

    void Update()
    {
        if (isHolding && !isZoomed && !dragHandler.IsDragging)
        {
            // �߰� ����: ������ ������� ������ Ȯ������ �ʵ��� ����
            if (cardDisplay != null && ManaManager.Instance != null &&
                ManaManager.Instance.currentMana < cardDisplay.cardData.cost)
                return;

            holdTimer += Time.unscaledDeltaTime;
            if (holdTimer >= holdTime)
            {
                ZoomInCard();
            }
        }

        if (isZoomed && Input.GetMouseButtonUp(0))
        {
            ZoomOutCard();
            isHolding = false;
            holdTimer = 0f;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f;
    }

    private void ZoomInCard()
    {
        isZoomed = true;
        Time.timeScale = 0.2f;

        if (zoomedCardInstance != null) Destroy(zoomedCardInstance);

        // ī�� ���� ����
        zoomedCardInstance = Instantiate(gameObject, parentCanvas.transform);
        zoomedCardInstance.transform.SetAsLastSibling();

        // RectTransform ����
        zoomedCardRect = zoomedCardInstance.GetComponent<RectTransform>();
        zoomedCardRect.anchorMin = new Vector2(0.5f, 0.5f);
        zoomedCardRect.anchorMax = new Vector2(0.5f, 0.5f);
        zoomedCardRect.pivot = new Vector2(0.5f, 0.5f);
        zoomedCardRect.anchoredPosition = Vector2.zero;
        zoomedCardInstance.transform.localScale = originalScale;

        // ���콺 �̺�Ʈ ����
        CanvasGroup cg = zoomedCardInstance.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        // Ȯ�� �ִϸ��̼�
        LeanTween.scale(zoomedCardInstance, originalScale * 2f, 0.25f).setEaseOutExpo();

        // �׵θ� ����Ʈ ����
        if (screenEdgeEffectPrefab != null && currentEffectInstance == null)
        {
            currentEffectInstance = Instantiate(screenEdgeEffectPrefab, parentCanvas.transform);
            currentEffectInstance.transform.SetAsLastSibling();

            RectTransform fxRect = currentEffectInstance.GetComponent<RectTransform>();
            if (fxRect != null)
            {
                fxRect.anchorMin = Vector2.zero;
                fxRect.anchorMax = Vector2.one;
                fxRect.offsetMin = Vector2.zero;
                fxRect.offsetMax = Vector2.zero;
            }
        }
    }

    private void ZoomOutCard()
    {
        isZoomed = false;
        Time.timeScale = 1f;

        if (zoomedCardInstance != null)
        {
            Destroy(zoomedCardInstance);
            zoomedCardInstance = null;
        }

        if (currentEffectInstance != null)
        {
            Destroy(currentEffectInstance);
            currentEffectInstance = null;
        }
    }
}