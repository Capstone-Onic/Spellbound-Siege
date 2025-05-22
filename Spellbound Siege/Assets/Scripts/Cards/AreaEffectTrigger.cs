using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound;

public class AreaEffectTrigger : MonoBehaviour
{
    public Card sourceCard;           // ���� ī�� ����
    public float duration = 3f;       // ��ü ���� �ð� (��)
    public float tickInterval = 0.2f; // ApplyEffect ȣ�� ���� (��)

    // �̹� ó���� ���� �ٽ� ó������ �ʱ� ���� ����
    private HashSet<EnemyController> affectedEnemies = new HashSet<EnemyController>();

    private void Start()
    {
        if (sourceCard == null)
        {
            Debug.LogError("[AreaEffectTrigger] sourceCard�� �������� �ʾҽ��ϴ�.");
            Destroy(gameObject);
            return;
        }

        StartCoroutine(ApplyEffectOverTime());
    }

    // duration ���� tickInterval���� ApplyEffectToEnemiesInRange ����
    private IEnumerator ApplyEffectOverTime()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            ApplyEffectToEnemiesInRange();
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(gameObject);
    }

    private void ApplyEffectToEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, sourceCard.effectRadius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyController>();
            if (enemy == null || affectedEnemies.Contains(enemy))
                continue;

            affectedEnemies.Add(enemy);

            // Ice �迭 ī�� (Ice Spear, Blizzard ��)
            if (sourceCard.cardType.Contains(Card.CardType.Ice))
            {
                // instantDamage: IceSpear = 0, Blizzard = 30 (SO���� ������ damage ��)
                float instantDamage = sourceCard.damage;
                StartCoroutine(ApplyDamageAndSlow(
                    enemy,
                    instantDamage,
                    0.5f,                           // ���ο� ����
                    sourceCard.statusEffectDuration // IceSpear=3, Blizzard=5 ��
                ));
            }
            // Fire �迭 ī�� (���� ��Ʈ)
            else if (sourceCard.cardType.Contains(Card.CardType.Fire))
            {
                float instantDamage = sourceCard.damage;
                StartCoroutine(ApplyBurnDamage(
                    enemy,
                    instantDamage,                // ƽ�� ������
                    0.5f,                         // ƽ ����
                    sourceCard.effectDuration     // SO�� ������ ���ӽð�
                ));
            }
            // �ʿ��ϴٸ� �ٸ� �Ӽ� �б� �߰���
        }
    }

    // ��� ����� + ���ο츦 ó���ϰ�, duration �� ���ο� ����
    private IEnumerator ApplyDamageAndSlow(
        EnemyController enemy,
        float instantDamage,
        float slowMultiplier,
        float duration)
    {
        // ��� �����
        if (instantDamage > 0f)
            enemy.TakeDamage(instantDamage);

        // ���ο� ����
        float originalSpeed = enemy.speedMultiplier;
        enemy.speedMultiplier = slowMultiplier;

        // ���� �ð� ���
        yield return new WaitForSeconds(duration);

        // �ӵ� ����
        if (enemy != null)
            enemy.speedMultiplier = originalSpeed;
    }

    
    // ���� �ð� ���� interval �������� ������
    private IEnumerator ApplyBurnDamage(
        EnemyController enemy,
        float damage,
        float interval,
        float duration)
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
}