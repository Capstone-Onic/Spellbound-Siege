using UnityEngine;
using UnityEngine.EventSystems;

public class CardDropTarget : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("3D DropZone에 카드 드롭됨!");

        var cardDisplay = eventData.pointerDrag?.GetComponent<CardDisplay>();
        if (cardDisplay != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GridTile tile = hit.collider.GetComponent<GridTile>();
                if (tile != null)
                {
                    if (ManaManager.Instance.ConsumeMana(cardDisplay.cardData.cost))
                    {
                        CardEffectProcessor.ApplyCardEffectToTile(cardDisplay.cardData, tile, tile.transform.position);
                        cardDisplay.UseCard(); // 카드 제거
                    }
                    else
                    {
                        Debug.Log("마나 부족으로 카드 사용 불가");
                        ManaManager.Instance.ShowManaWarning();
                    }
                }
            }
        }
    }
}