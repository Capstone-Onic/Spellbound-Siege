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
        transform.SetParent(originalParent.root); // �ֻ����� �̵��Ͽ� UI ��ħ ����
        canvasGroup.alpha = 0.6f; // ������ ȿ��
        canvasGroup.blocksRaycasts = false; // �ٸ� UI ���� �����ϰ� ����
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition; // ���콺 ��ġ�� �̵�
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Ư�� ����(��: �ʵ�)�� ���� ��� ī�� ���
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("DropZone"))
        {
            UseCard();
        }
        else
        {
            // ���� ��ġ�� ���ư�
            transform.position = originalPosition;
            transform.SetParent(originalParent);
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void UseCard()
    {
        Debug.Log($"ī�� ����: {gameObject.name}");
        Destroy(gameObject); // ī�� ��� �� ����
    }
}