using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spellbound
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public List<CardType> cardType;
        public int cost;
        public int damageMin;
        public int damageMax;
        public List<DamageType> damageType;

        [Header("���� ����")]
        [Tooltip("�� ī�带 ����� �� ȿ���� ����Ǵ� ����(������)�Դϴ�.")]
        public float effectRadius = 1.5f;

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
    }
}