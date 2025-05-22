using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public static class CardEffectProcessor
{
    // ���� ����Ʈ ������
    public static GameObject fireballEffect;
    public static GameObject firewallEffect;
    public static GameObject icespearEffect;
    public static GameObject stoneEffect;
    public static GameObject watershotEffect;

    // �Ӽ��� ���� ����Ʈ ��Ͽ� ��ųʸ�
    private static Dictionary<Card.CardType, GameObject> statusEffectMap = new();

    // ��� �Լ�: GameInitializer���� ȣ��
    public static void RegisterStatusEffect(Card.CardType type, GameObject prefab)
    {
        if (!statusEffectMap.ContainsKey(type))
            statusEffectMap.Add(type, prefab);
    }

    // ī�� ȿ�� ����
    public static void ApplyCardEffectToTile(Card cardData, GridTile unused, Vector3 position)
    {
        if (cardData.deliveryType == Card.EffectDeliveryType.Falling)
        {
            // ������ �� ����Ʈ�� ����, ���ش� FallingEffectController���� ����
            SpawnParticleEffect(position, cardData);
        }
        else if (cardData.deliveryType == Card.EffectDeliveryType.GroundGrow)
        {
            // GroundGrow �� ����Ʈ + ���� ��� ����
            SpawnParticleEffect(position, cardData);
            ApplyDamageAndStatus(cardData, position);
        }
    }

    // �� ����
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

    // ���� ����Ʈ ����
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
                return; // �� Ÿ�Ը� ���� (�ʿ� �� ���� ����)
            }
        }
    }

    // ȭ�� ���� (Burn)
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

    // ���ο�
    private static IEnumerator ApplySlow(EnemyController enemy, float slowRate, float duration)
    {
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = slowRate;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    // ����
    private static IEnumerator ApplyStun(EnemyController enemy, float duration)
    {
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = 0f;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    // ī�� ����Ʈ ������ ����
    private static void SpawnParticleEffect(Vector3 position, Card cardData)
    {
        if (cardData.deliveryType == Card.EffectDeliveryType.Falling)
        {
            if (cardData.fallEffectPrefab == null || cardData.impactEffectPrefab == null)
            {
                Debug.LogWarning($"[CardEffectProcessor] ������ ī�忡 �ʿ��� �������� �����ϴ�: {cardData.cardName}");
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
                Debug.LogWarning($"[CardEffectProcessor] GroundGrow ī�忡 ���� ����Ʈ �������� �����ϴ�: {cardData.cardName}");
                return;
            }

            Vector3 spawnPos = position + Vector3.up * 0.1f;
            Quaternion prefabRotation = cardData.impactEffectPrefab.transform.rotation;

            GameObject fx = GameObject.Instantiate(cardData.impactEffectPrefab, spawnPos, prefabRotation);

            // impactEffectPrefab�� �⺻ Y �������� ����, XZ�� radius * 2
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

            if (name.Contains("ȭ����"))
            {
                enemy.TakeDamage(15f);
                enemy.StartCoroutine(ApplyBurnDamage(enemy, 3f, 0.5f, cardData.statusEffectDuration));
            }
            else if (name.Contains("����â"))
            {
                enemy.TakeDamage(15f);
                enemy.StartCoroutine(ApplySlow(enemy, 0.5f, cardData.effectDuration));
            }
            else if (name.Contains("����"))
            {
                enemy.TakeDamage(13f);
                enemy.StartCoroutine(ApplyStun(enemy, 1.0f));
            }
            else if (name.Contains("���ͼ�"))
            {
                enemy.TakeDamage(25f);
            }

            AttachStatusEffect(cardData, enemy, cardData.statusEffectDuration);
        }
    }
}