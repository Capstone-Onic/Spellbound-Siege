using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public static class CardEffectProcessor
{
    // 개별 이펙트 프리팹
    public static GameObject fireballEffect;
    public static GameObject firewallEffect;
    public static GameObject icespearEffect;
    public static GameObject stoneEffect;
    public static GameObject watershotEffect;

    // 속성별 상태 이펙트 등록용 딕셔너리
    private static Dictionary<Card.CardType, GameObject> statusEffectMap = new();

    // 등록 함수: GameInitializer에서 호출
    public static void RegisterStatusEffect(Card.CardType type, GameObject prefab)
    {
        if (!statusEffectMap.ContainsKey(type))
            statusEffectMap.Add(type, prefab);
    }

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
                GameObject.Destroy(fx, cardData.statusEffectDuration);
                return; // 한 타입만 적용 (필요 시 제거 가능)
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

    // 카드 이펙트 프리팹 생성
    private static void SpawnParticleEffect(Vector3 position, Card cardData)
    {
        if (cardData.deliveryType == Card.EffectDeliveryType.Falling)
        {
            if (cardData.fallEffectPrefab == null || cardData.impactEffectPrefab == null)
            {
                Debug.LogWarning($"[CardEffectProcessor] 낙하형 카드에 필요한 프리팹이 없습니다: {cardData.cardName}");
                return;
            }

            Vector3 startPos = position + Vector3.up * 3f;
            Quaternion prefabRotation = cardData.fallEffectPrefab.transform.rotation;

            GameObject fx = GameObject.Instantiate(cardData.fallEffectPrefab, startPos, prefabRotation);

            var falling = fx.AddComponent<FallingEffectController>();
            falling.Initialize(position, cardData, cardData.impactEffectPrefab);
        }
        else if (cardData.deliveryType == Card.EffectDeliveryType.GroundGrow)
        {
            if (cardData.impactEffectPrefab == null)
            {
                Debug.LogWarning($"[CardEffectProcessor] GroundGrow 카드에 폭발 이펙트 프리팹이 없습니다: {cardData.cardName}");
                return;
            }

            Vector3 spawnPos = position + Vector3.up * 0.1f;
            Quaternion prefabRotation = cardData.impactEffectPrefab.transform.rotation;

            GameObject fx = GameObject.Instantiate(cardData.impactEffectPrefab, spawnPos, prefabRotation);

            // impactEffectPrefab의 기본 Y 스케일은 유지, XZ만 radius * 2
            float radius = cardData.effectRadius;
            Vector3 baseScale = cardData.impactEffectPrefab.transform.localScale;
            Vector3 targetScale = new Vector3(radius * 2f, baseScale.y, radius * 2f);

            fx.transform.localScale = Vector3.zero;
            LeanTween.scale(fx, targetScale, 0.5f).setEaseOutExpo();

            ApplyDamageAndStatus(cardData, position);
            GameObject.Destroy(fx, 1.5f);
        }
    }

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
                enemy.StartCoroutine(ApplySlow(enemy, 0.5f, cardData.effectDuration));
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
}