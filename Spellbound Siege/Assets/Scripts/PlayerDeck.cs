using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public List<Card> deck = new List<Card>(); // 덱에 카드 목록 저장
    public int energy = 10;

    public void PlayCard(Card card, Vector3 position)
    {
        if (energy >= card.cost)
        {
            card.UseCard(position);
            energy -= card.cost;
        }
        else
        {
            Debug.Log("Not enough energy!");
        }
    }
}
