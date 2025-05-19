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

        [Header("상태 효과 지속시간")]
        public float statusEffectDuration = 3f;

        [TextArea(2, 5)]
        public string description;  // 카드 설명

        [Header("이펙트 프리팹")]
        public GameObject fallEffectPrefab;   // 낙하 이펙트 (위에서 떨어지는 이펙트)
        public GameObject impactEffectPrefab; // 바닥에 도달한 후 폭발 이펙트

        [Header("이펙트 유형")]
        public EffectDeliveryType deliveryType = EffectDeliveryType.Falling;

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

        public enum EffectDeliveryType
        {
            Falling,    // 하늘에서 낙하 후 폭발
            GroundGrow  // 바닥에서 즉시 시작하여 확장
        }
    }
}