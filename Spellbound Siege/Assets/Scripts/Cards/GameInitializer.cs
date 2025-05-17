using UnityEngine;
using System.Collections.Generic;
using Spellbound;

public class GameInitializer : MonoBehaviour
{
    [Header("�⺻ ī�� ����Ʈ ������")]
    public GameObject fireballEffect;
    public GameObject firewallEffect;
    public GameObject icespearEffect;
    public GameObject stoneEffect;
    public GameObject watershotEffect;

    [System.Serializable]
    public class CardEffectEntry
    {
        public Card.CardType type;
        public GameObject effectPrefab;
    }

    [Header("�Ӽ��� ���� ����Ʈ ������")]
    public List<CardEffectEntry> statusEffectMappings = new();

    void Start()
    {
        // �⺻ ����Ʈ ����
        CardEffectProcessor.fireballEffect = fireballEffect;
        CardEffectProcessor.firewallEffect = firewallEffect;
        CardEffectProcessor.icespearEffect = icespearEffect;
        CardEffectProcessor.stoneEffect = stoneEffect;
        CardEffectProcessor.watershotEffect = watershotEffect;

        // �Ӽ��� ���� ����Ʈ ���
        foreach (var entry in statusEffectMappings)
        {
            CardEffectProcessor.RegisterStatusEffect(entry.type, entry.effectPrefab);
        }
    }
}