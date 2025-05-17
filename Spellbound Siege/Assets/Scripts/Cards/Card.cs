using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spellbound
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string cardName;
        public Sprite cardImage;
        public List<CardType> cardType;
        public int cost;
        public int damageMin;
        public int damageMax;
        public List<DamageType> damageType;

        [Header("범위 설정")]
        [Tooltip("이 카드를 사용할 때 효과가 적용되는 범위(반지름)입니다.")]
        public float effectRadius = 1.5f;
        
        [Header("지속 효과")]
        public float effectDuration = 3f;

        [TextArea(2, 5)]
        public string description;  // 카드 설명

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