using System.Collections;
using UnityEngine;
using Spellbound;

public static class CardEffectProcessor
{
    public static GameObject defaultEffectPrefab;

    public static void ApplyCardEffectToTile(Card cardData, GridTile unused, Vector3 position)
    {
        Color flashColor = Color.white;

        foreach (var type in cardData.damageType)
        {
            switch (type)
            {
                case Card.DamageType.Fire:
                    ApplyBurn(position);
                    flashColor = Color.red;
                    break;
                case Card.DamageType.Water:
                    ApplyWater(position);
                    flashColor = Color.blue;
                    break;
                case Card.DamageType.Ice:
                    ApplyIce(position);
                    flashColor = Color.cyan;
                    break;
            }
        }

        // 이펙트 표시
        ShowEffect(position, flashColor);

        // 색상 플래시
        Collider[] hits = Physics.OverlapSphere(position, 0.4f);
        foreach (var hit in hits)
        {
            var flash = hit.GetComponent<ColorFlashEffect>();
            if (flash != null)
            {
                flash.FlashColor(flashColor);
            }
        }
    }

    private static void ApplyWater(Vector3 pos)
    {
        var enemy = GetEnemyAtPosition(pos);
        if (enemy != null)
        {
            enemy.TakeDamage(8f);
            Debug.Log("물 데미지 적용");
        }
    }

    private static void ApplyBurn(Vector3 pos)
    {
        var enemy = GetEnemyAtPosition(pos);
        if (enemy != null)
        {
            enemy.StartCoroutine(ApplyBurnDamage(enemy, 2f, 0.5f, 3f));
            Debug.Log("지속 피해 시작");
        }
    }

    private static void ApplyIce(Vector3 pos)
    {
        var enemy = GetEnemyAtPosition(pos);
        if (enemy != null)
        {
            enemy.StartCoroutine(ApplySlow(enemy, 0.3f, 2f));
            Debug.Log("슬로우 적용");
        }
    }

    private static EnemyController GetEnemyAtPosition(Vector3 pos)
    {
        Collider[] hits = Physics.OverlapSphere(pos, 0.4f);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy != null) return enemy;
        }
        return null;
    }

    private static EnemyController GetEnemyOnTile(GridTile tile)
    {
        Collider[] hits = Physics.OverlapSphere(tile.transform.position, 0.4f);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
                return enemy;
        }
        return null;
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

    private static void ShowEffect(Vector3 position, Color color)
    {
        GameObject effect;

        if (defaultEffectPrefab != null)
        {
            // 지정된 프리팹 사용
            effect = GameObject.Instantiate(defaultEffectPrefab, position + Vector3.up * 0.5f, Quaternion.identity);
        }
        else
        {
            // 기본 Sphere 이펙트 사용
            effect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            effect.transform.position = position + Vector3.up * 0.5f;
            effect.transform.localScale = Vector3.one * 1.5f;

            Renderer r = effect.GetComponent<Renderer>();
            r.material = new Material(Shader.Find("Standard"));
            r.material.color = color;
        }

        GameObject.Destroy(effect, 1.5f);
    }
}