using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public static class CardEffectProcessor
{
    public static GameObject defaultEffectPrefab;
    public static GameObject fireballEffect;
    public static GameObject firewallEffect;
    public static GameObject icespearEffect;
    public static GameObject stoneEffect;
    public static GameObject watershotEffect;

    public static void ApplyCardEffectToTile(Card cardData, GridTile unused, Vector3 position)
    {
        Color flashColor = Color.white;
        float radius = cardData.effectRadius;

        List<EnemyController> enemies = GetEnemiesInRange(position, radius);

        foreach (var enemy in enemies)
        {
            string name = cardData.cardName.ToLower(); // 소문자로 비교

            if (name.Contains("fire ball"))
            {
                enemy.TakeDamage(15f); // 데미지
                enemy.StartCoroutine(ApplyBurnDamage(enemy, 6f, 0.5f, 3f)); // 도트 데미지
                SpawnParticleEffect(enemy.transform.position, cardData.cardName);
            }
            else if (name.Contains("fire wall"))
            {
                enemy.StartCoroutine(ApplyBurnDamage(enemy, 6f, 0.5f, 3f)); // 도트 데미지
                SpawnParticleEffect(enemy.transform.position, cardData.cardName);
            }
            else if (name.Contains("ice spear"))
            {
                enemy.TakeDamage(17f); // 데미지
                enemy.StartCoroutine(ApplySlow(enemy, 0.5f, 2f)); // 슬로우
                SpawnParticleEffect(enemy.transform.position, cardData.cardName);
            }
            else if (name.Contains("stone"))
            {
                enemy.TakeDamage(13f); // 데미지
                enemy.StartCoroutine(ApplyStun(enemy, 1.0f)); // 스턴
                SpawnParticleEffect(enemy.transform.position, cardData.cardName);
            }
            else if (name.Contains("water shot"))
            {
                enemy.TakeDamage(25f); // 데미지
                SpawnParticleEffect(enemy.transform.position, cardData.cardName);
            }

            var flash = enemy.GetComponent<ColorFlashEffect>();
            if (flash != null)
                flash.FlashColor(flashColor);
        }

        ShowEffect(position, flashColor);
    }

    private static List<EnemyController> GetEnemiesInRange(Vector3 pos, float radius)
    {
        List<EnemyController> enemies = new List<EnemyController>();
        Collider[] hits = Physics.OverlapSphere(pos, radius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
        return enemies;
    }

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

    private static IEnumerator ApplySlow(EnemyController enemy, float slowRate, float duration)
    {
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = slowRate;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    private static IEnumerator ApplyStun(EnemyController enemy, float duration)
    {
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = 0f;
        yield return new WaitForSeconds(duration);
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    private static void ShowEffect(Vector3 position, Color color)
    {
        if (defaultEffectPrefab != null)
        {
            GameObject effect = GameObject.Instantiate(defaultEffectPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
            GameObject.Destroy(effect, 1.5f);
        }
    }

    private static void SpawnParticleEffect(Vector3 position, string cardName)
    {
        GameObject prefab = null;
        string name = cardName.ToLower(); // 비교를 위해 소문자 처리

        if (name.Contains("fire ball"))
            prefab = fireballEffect;
        else if (name.Contains("fire wall"))
            prefab = firewallEffect;
        else if (name.Contains("ice spear"))
            prefab = icespearEffect;
        else if (name.Contains("stone"))
            prefab = stoneEffect;
        else if (name.Contains("water shot"))
            prefab = watershotEffect;

        if (prefab != null)
        {
            GameObject fx = GameObject.Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
            GameObject.Destroy(fx, 2f);
        }
    }
}