using UnityEngine;
using System.Collections.Generic;
using Spellbound;

public class GameInitializer : MonoBehaviour
{
    [Header("����")]
    public AudioSource globalAudioSource;

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
        // ���� ����
        CardEffectProcessor.globalAudioSource = globalAudioSource;

        // �Ӽ��� ���� ����Ʈ ���
        foreach (var entry in statusEffectMappings)
        {
            CardEffectProcessor.RegisterStatusEffect(entry.type, entry.effectPrefab);
        }
    }
}