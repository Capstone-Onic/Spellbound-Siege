using UnityEngine;

public class ClickableUnit : MonoBehaviour
{
    private GridTile parentTile;

    public void SetParentTile(GridTile tile)
    {
        parentTile = tile;
    }

    private void OnMouseDown()
    {
        if (!StartGameManager.gameStarted && parentTile != null)
        {
            parentTile.ClearUnit();
        }
    }
}