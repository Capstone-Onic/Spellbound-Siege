using UnityEngine;
using UnityEngine.EventSystems;

public class CardDropTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("3D DropZone�� ī�� ��ӵ�!");

        var card = eventData.pointerDrag?.GetComponent<CardDisplay>();
        if (card != null)
        {
            card.UseCard(); // ī�� ���� �Ǵ� ȿ�� ó��
        }
    }
}