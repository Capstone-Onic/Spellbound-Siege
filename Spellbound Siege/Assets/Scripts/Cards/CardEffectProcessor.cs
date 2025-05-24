using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public static class CardEffectProcessor
{
    // 속성별 상태 이펙트 등록용 딕셔너리
    private static Dictionary<Card.CardType, GameObject> statusEffectMap = new();

    // 카드 효과 적용
    public static void ApplyCardEffectToTile(Card cardData, GridTile unused, Vector3 position)
    {
        if (cardData.deliveryType == Card.EffectDeliveryType.Falling)
        {
            // 낙하형 → 이펙트만 생성, 피해는 FallingEffectController에서 적용
            SpawnParticleEffect(position, cardData);
        }
        else if (cardData.deliveryType == Card.EffectDeliveryType.GroundGrow)
        {
            // GroundGrow → 이펙트 + 피해 즉시 적용
            SpawnParticleEffect(position, cardData);
            ApplyDamageAndStatus(cardData, position);
        }
    }

    // 적 감지
    private static List<EnemyController> GetEnemiesInRange(Vector3 pos, float radius)
    {
        List<EnemyController> enemies = new List<EnemyController>();
        Collider[] hits = Physics.OverlapSphere(pos, radius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
                enemies.Add(enemy);
        }
        return enemies;
    }

    // 상태 이펙트 부착
    private static void AttachStatusEffect(Card cardData, EnemyController enemy, float duration)
    {
        if (enemy == null || enemy.GetComponent<Renderer>() == null) return;

        foreach (var type in cardData.cardType)
        {
            if (statusEffectMap.TryGetValue(type, out GameObject prefab) && prefab != null)
            {
                GameObject fx = GameObject.Instantiate(prefab, enemy.transform);
                float height = enemy.GetComponent<Renderer>().bounds.size.y;
                fx.transform.localPosition = new Vector3(0, height * 0.3f, 0);
                GameObject.Destroy(fx, duration);
                return; // 한 타입만 적용
            }
        }
    }

    // 화상 피해 (Burn)
    private static IEnumerator ApplyBurnDamage(EnemyController enemy, float damage, float interval, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (enemy == null) yield break;
            enemy.TakeDamage(damage);
            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }
    }

    // 슬로우
    private static IEnumerator ApplySlow(EnemyController enemy, float slowRate, float duration)
    {
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = slowRate;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    // 스턴
    private static IEnumerator ApplyStun(EnemyController enemy, float duration)
    {
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = 0f;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    // 카드 이펙트 프리팹 생성 및 사운드 동기화
    public static void SpawnParticleEffect(Vector3 position, Card cardData)
    {
        GameObject prefabToSpawn;
        AudioClip clipToPlay;
        Vector3 spawnPos;
        Quaternion spawnRot;

        switch (cardData.deliveryType)
        {
            case Card.EffectDeliveryType.Falling:
                if (cardData.fallEffectPrefab == null || cardData.impactEffectPrefab == null)
                {
                    Debug.LogWarning($"[CardEffectProcessor] 낙하형 카드에 필요한 프리팹이 없습니다: {cardData.cardName}");
                    return;
                }

                prefabToSpawn = cardData.fallEffectPrefab;
                clipToPlay = cardData.fallSound;
                spawnPos = position + Vector3.up * 3f;
                spawnRot = prefabToSpawn.transform.rotation;

                GameObject fallingGO = Object.Instantiate(prefabToSpawn, spawnPos, spawnRot);

                // 낙하 초기화 호출
                var controller = fallingGO.GetComponent<FallingEffectController>();
                if (controller != null)
                {
                    controller.Initialize(position, cardData, cardData.impactEffectPrefab);
                }
                else
                {
                    Debug.LogWarning($"[CardEffectProcessor] {fallingGO.name}에 FallingEffectController가 없습니다.");
                }

                // 낙하 사운드 재생
                var fallAudio = fallingGO.AddComponent<TimedAudioPlayer>();
                fallAudio.PlayClip(clipToPlay, cardData);

                return; // GroundGrow와 중복 실행 방지

            case Card.EffectDeliveryType.GroundGrow:
                if (cardData.impactEffectPrefab == null)
                {
                    Debug.LogWarning($"[CardEffectProcessor] GroundGrow 카드에 폭발 이펙트 프리팹이 없습니다: {cardData.cardName}");
                    return;
                }

                prefabToSpawn = cardData.impactEffectPrefab;
                clipToPlay = cardData.impactSound;
                spawnPos = position + Vector3.up * 0.1f;
                spawnRot = prefabToSpawn.transform.rotation;

                GameObject groundGO = Object.Instantiate(prefabToSpawn, spawnPos, spawnRot);

                var ps = groundGO.GetComponent<ParticleSystem>() ?? groundGO.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.stopAction = ParticleSystemStopAction.Destroy;
                    ps.Play();
                }
                else
                {
                    Debug.LogWarning($"[CardEffectProcessor] {groundGO.name}에 ParticleSystem이 없습니다.");
                }

                var growAudio = groundGO.AddComponent<TimedAudioPlayer>();
                growAudio.PlayClip(clipToPlay, cardData);

                // 피해/상태 효과는 GroundGrow 즉시 적용
                ApplyDamageAndStatus(cardData, position);
                break;

            default:
                Debug.LogWarning($"[CardEffectProcessor] 알 수 없는 deliveryType입니다: {cardData.deliveryType}");
                return;
        }
    }

    // 데미지 및 상태효과 적용
    public static void ApplyDamageAndStatus(Card cardData, Vector3 position)
    {
        float radius = cardData.effectRadius;
        List<EnemyController> enemies = GetEnemiesInRange(position, radius);

        foreach (var enemy in enemies)
        {
            string name = cardData.cardName.ToLower();

            if (name.Contains("화염구"))
            {
                enemy.TakeDamage(15f);
                enemy.StartCoroutine(ApplyBurnDamage(enemy, 3f, 0.5f, cardData.statusEffectDuration));
            }
            else if (name.Contains("얼음창"))
            {
                enemy.TakeDamage(15f);
                enemy.StartCoroutine(ApplySlow(enemy, 0.5f, cardData.statusEffectDuration));
            }
            else if (name.Contains("낙석"))
            {
                enemy.TakeDamage(13f);
                enemy.StartCoroutine(ApplyStun(enemy, 1.0f));
            }
            else if (name.Contains("워터샷"))
            {
                enemy.TakeDamage(25f);
            }

            AttachStatusEffect(cardData, enemy, cardData.statusEffectDuration);
        }
    }

    // (기존에 남겨두어도 무방합니다)
    public static AudioSource globalAudioSource;
    public static void PlaySound(AudioClip clip)
    {
        if (clip != null && globalAudioSource != null)
            globalAudioSource.PlayOneShot(clip);
    }

    // 등록 함수: GameInitializer에서 호출
    public static void RegisterStatusEffect(Card.CardType type, GameObject prefab)
    {
        if (!statusEffectMap.ContainsKey(type))
            statusEffectMap.Add(type, prefab);
    }
}