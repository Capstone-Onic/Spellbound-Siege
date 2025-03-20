using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public int cost;
    public GameObject unitPrefab;

    public void UseCard(Vector3 position)
    {
        if (unitPrefab != null)
        {
            GameObject.Instantiate(unitPrefab, position, Quaternion.identity);
        }
    }
}