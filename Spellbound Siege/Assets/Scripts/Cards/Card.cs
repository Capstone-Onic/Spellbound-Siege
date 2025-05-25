using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spellbound
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
    public class Card : ScriptableObject, System.IEquatable<Card>
    {
        public string cardName;
        public Sprite cardImage;
        public List<CardType> cardType;
        public int cost;
        public int damage;
        public List<DamageType> damageType;

        [Header("범위 설정")]
        [Tooltip("이 카드를 사용할 때 효과가 적용되는 범위(반지름)입니다.")]
        public float effectRadius = 1.5f;

        [Header("지속 효과")]
        public float effectDuration = 3f; // 이펙트 지속시간

        [Header("상태 효과 지속시간")]
        public float statusEffectDuration = 3f;

        [TextArea(2, 5)]
        public string description;  // 카드 설명

        [Header("이펙트 프리팹")]
        public GameObject fallEffectPrefab;
        public GameObject impactEffectPrefab;

        [Header("이펙트 유형")]
        public EffectDeliveryType deliveryType = EffectDeliveryType.Falling;

        [Header("이펙트 사운드")]
        public AudioClip fallSound;
        public AudioClip impactSound;

        [Header("잠금 상태")]
        public bool isUnlockedByDefault = false;

        public enum CardType
        {
            Fire, Water, Ice, Earth, Light, Dark
        }

        public enum DamageType
        {
            Fire, Water, Ice, Earth, Light, Dark
        }

        public enum EffectDeliveryType
        {
            Falling,
            GroundGrow
        }

        // 카드 비교 로직 강화
        public bool Equals(Card other)
        {
            if (other == null) return false;
            return this.cardName == other.cardName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Card);
        }

        public override int GetHashCode()
        {
            return cardName != null ? cardName.GetHashCode() : 0;
        }
    }
}