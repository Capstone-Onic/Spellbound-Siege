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
        Color flashColor = Color.white;
        float radius = cardData.effectRadius;

        List<EnemyController> enemies = GetEnemiesInRange(position, radius);

        foreach (var enemy in enemies)
        {
            string name = cardData.cardName.ToLower(); // 카드 이름 기준 분기

            if (name.Contains("fire ball"))
            {
                enemy.TakeDamage(15f);
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    enemy.StartCoroutine(ApplyBurnDamage(enemy, 3f, 0.5f, 3f));
            }
            else if (name.Contains("fire field"))
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    enemy.StartCoroutine(ApplyBurnDamage(enemy, 6f, 0.5f, 3f));
            }
            else if (name.Contains("ice spear"))
            {
                enemy.TakeDamage(15f);
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    enemy.StartCoroutine(ApplySlow(enemy, 0.5f, 2f));
            }
            else if (name.Contains("stone"))
            {
                enemy.TakeDamage(13f);
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    enemy.StartCoroutine(ApplyStun(enemy, 1.0f));
            }
            else if (name.Contains("water shot"))
            {
                enemy.TakeDamage(25f);
            }

            //  속성 기반 공통 상태 이펙트 부착
            AttachStatusEffect(cardData, enemy, cardData.effectDuration);
        }

        // 시각 이펙트 생성
        SpawnParticleEffect(position, cardData);
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
        GameObject prefab = null;
        string name = cardData.cardName.ToLower();

        if (name.Contains("fire ball"))
        {
            prefab = fireballEffect;
        }
        else if (name.Contains("fire field"))
        {
            prefab = firewallEffect;
        }
        else if (name.Contains("ice spear"))
        {
            prefab = icespearEffect;
        }
        else if (name.Contains("stone"))
        {
            prefab = stoneEffect;
        }
        else if (name.Contains("water shot"))
        {
            prefab = watershotEffect;
        }

        if (prefab != null)
        {
            GameObject fx = GameObject.Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);

            var area = fx.GetComponent<AreaEffectTrigger>();
            if (area != null)
            {
                area.sourceCard = cardData;
                area.duration = cardData.effectDuration;
            }
            else
            {
                GameObject.Destroy(fx, 1f); // 즉발형 이펙트는 1초 후 제거
            }
        }
    }
}