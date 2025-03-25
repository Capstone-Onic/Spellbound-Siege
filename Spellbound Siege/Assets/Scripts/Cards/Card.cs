using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public List<CardType> cardType;
    public int cost;
    public int dmgMin;
    public int dmgMax;
    public List<DamageType> damageType;
    public GameObject unitPrefab;

    public enum CardType
    {
        Fire,
        Water,
        Ice,
        Earth,
        Light,
        Dark
    }

    public enum DamageType
    {
        Fire,
        Water,
        Ice,
        Earth,
        Light,
        Dark
    }

    public void UseCard(Vector3 position)
    {
        if (unitPrefab != null)
        {
            GameObject.Instantiate(unitPrefab, position, Quaternion.identity);
        }
    }
}