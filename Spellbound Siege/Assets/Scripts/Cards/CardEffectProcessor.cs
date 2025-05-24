using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public static class CardEffectProcessor
{
    // �Ӽ��� ���� ����Ʈ ��Ͽ� ��ųʸ�
    private static Dictionary<Card.CardType, GameObject> statusEffectMap = new();

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
                GameObject.Destroy(fx, duration);
                return; // �� Ÿ�Ը� ����
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

    // ī�� ����Ʈ ������ ���� �� ���� ����ȭ
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
                    Debug.LogWarning($"[CardEffectProcessor] ������ ī�忡 �ʿ��� �������� �����ϴ�: {cardData.cardName}");
                    return;
                }

                prefabToSpawn = cardData.fallEffectPrefab;
                clipToPlay = cardData.fallSound;
                spawnPos = position + Vector3.up * 3f;
                spawnRot = prefabToSpawn.transform.rotation;

                GameObject fallingGO = Object.Instantiate(prefabToSpawn, spawnPos, spawnRot);

                // ���� �ʱ�ȭ ȣ��
                var controller = fallingGO.GetComponent<FallingEffectController>();
                if (controller != null)
                {
                    controller.Initialize(position, cardData, cardData.impactEffectPrefab);
                }
                else
                {
                    Debug.LogWarning($"[CardEffectProcessor] {fallingGO.name}�� FallingEffectController�� �����ϴ�.");
                }

                // ���� ���� ���
                var fallAudio = fallingGO.AddComponent<TimedAudioPlayer>();
                fallAudio.PlayClip(clipToPlay, cardData);

                return; // GroundGrow�� �ߺ� ���� ����

            case Card.EffectDeliveryType.GroundGrow:
                if (cardData.impactEffectPrefab == null)
                {
                    Debug.LogWarning($"[CardEffectProcessor] GroundGrow ī�忡 ���� ����Ʈ �������� �����ϴ�: {cardData.cardName}");
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
                    Debug.LogWarning($"[CardEffectProcessor] {groundGO.name}�� ParticleSystem�� �����ϴ�.");
                }

                var growAudio = groundGO.AddComponent<TimedAudioPlayer>();
                growAudio.PlayClip(clipToPlay, cardData);

                // ����/���� ȿ���� GroundGrow ��� ����
                ApplyDamageAndStatus(cardData, position);
                break;

            default:
                Debug.LogWarning($"[CardEffectProcessor] �� �� ���� deliveryType�Դϴ�: {cardData.deliveryType}");
                return;
        }
    }

    // ������ �� ����ȿ�� ����
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
                enemy.StartCoroutine(ApplySlow(enemy, 0.5f, cardData.statusEffectDuration));
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

    // (������ ���ܵξ �����մϴ�)
    public static AudioSource globalAudioSource;
    public static void PlaySound(AudioClip clip)
    {
        if (clip != null && globalAudioSource != null)
            globalAudioSource.PlayOneShot(clip);
    }

    // ��� �Լ�: GameInitializer���� ȣ��
    public static void RegisterStatusEffect(Card.CardType type, GameObject prefab)
    {
        if (!statusEffectMap.ContainsKey(type))
            statusEffectMap.Add(type, prefab);
    }
}