using UnityEngine;
using UnityEngine.EventSystems;

public class CardDropTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("3D DropZone에 카드 드롭됨!");

        var card = eventData.pointerDrag?.GetComponent<CardDisplay>();
        if (card != null)
        {
            card.UseCard(); // 카드 제거 또는 효과 처리
        }
    }
}