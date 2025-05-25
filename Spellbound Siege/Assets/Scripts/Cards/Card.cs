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

        [Header("���� ����")]
        [Tooltip("�� ī�带 ����� �� ȿ���� ����Ǵ� ����(������)�Դϴ�.")]
        public float effectRadius = 1.5f;

        [Header("���� ȿ��")]
        public float effectDuration = 3f; // ����Ʈ ���ӽð�

        [Header("���� ȿ�� ���ӽð�")]
        public float statusEffectDuration = 3f;

        [TextArea(2, 5)]
        public string description;  // ī�� ����

        [Header("����Ʈ ������")]
        public GameObject fallEffectPrefab;
        public GameObject impactEffectPrefab;

        [Header("����Ʈ ����")]
        public EffectDeliveryType deliveryType = EffectDeliveryType.Falling;

        [Header("����Ʈ ����")]
        public AudioClip fallSound;
        public AudioClip impactSound;

        [Header("��� ����")]
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

        // ī�� �� ���� ��ȭ
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