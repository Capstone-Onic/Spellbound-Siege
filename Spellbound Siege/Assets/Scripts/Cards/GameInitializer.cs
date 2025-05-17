using UnityEngine;
using System.Collections.Generic;
using Spellbound;

public class GameInitializer : MonoBehaviour
{
    [Header("기본 카드 이펙트 프리팹")]
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

    [Header("속성별 상태 이펙트 프리팹")]
    public List<CardEffectEntry> statusEffectMappings = new();

    void Start()
    {
        // 기본 이펙트 연결
        CardEffectProcessor.fireballEffect = fireballEffect;
        CardEffectProcessor.firewallEffect = firewallEffect;
        CardEffectProcessor.icespearEffect = icespearEffect;
        CardEffectProcessor.stoneEffect = stoneEffect;
        CardEffectProcessor.watershotEffect = watershotEffect;

        // 속성별 상태 이펙트 등록
        foreach (var entry in statusEffectMappings)
        {
            CardEffectProcessor.RegisterStatusEffect(entry.type, entry.effectPrefab);
        }
    }
}