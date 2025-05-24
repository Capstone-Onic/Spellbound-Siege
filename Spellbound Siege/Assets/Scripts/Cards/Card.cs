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
        public GameObject fallEffectPrefab;   // ���� ����Ʈ (������ �������� ����Ʈ)
        public GameObject impactEffectPrefab; // �ٴڿ� ������ �� ���� ����Ʈ

        [Header("����Ʈ ����")]
        public EffectDeliveryType deliveryType = EffectDeliveryType.Falling;

        [Header("����Ʈ ����")]
        public AudioClip fallSound;    // ���� �� ���
        public AudioClip impactSound;  // ���� �� ���

        [Header("��� ����")]
        public bool isUnlockedByDefault = false; //false ���¸� ���

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
            Falling,    // �ϴÿ��� ���� �� ����
            GroundGrow  // �ٴڿ��� ��� �����Ͽ� Ȯ��
        }
    }
}