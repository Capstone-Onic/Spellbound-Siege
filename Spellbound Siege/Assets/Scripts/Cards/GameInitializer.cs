using UnityEngine;
using System.Collections.Generic;
using Spellbound;

public class GameInitializer : MonoBehaviour
{
    [Header("사운드")]
    public AudioSource globalAudioSource;

    [System.Serializable]
    public class CardEffectEntry
    {
        public Card.CardType type;
        public GameObject effectPrefab;
    }

    [Header("속성별 상태 이펙트 프리팹")]
    public List<CardEffectEntry> statusEffectMappings = new();

    void Start()
    {
        // 사운드 연결
        CardEffectProcessor.globalAudioSource = globalAudioSource;

        // 속성별 상태 이펙트 등록
        foreach (var entry in statusEffectMappings)
        {
            CardEffectProcessor.RegisterStatusEffect(entry.type, entry.effectPrefab);
        }
    }
}