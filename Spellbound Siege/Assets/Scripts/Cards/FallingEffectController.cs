using UnityEngine;
using System.Collections;
using Spellbound;

public class FallingEffectController : MonoBehaviour
{
    public Card cardData;
    public GameObject impactEffectPrefab;
    public float fallDuration = 0.5f;

    private Vector3 targetPosition;

    public void Initialize(Vector3 target, Card card, GameObject impactEffect)
    {
        targetPosition = target + Vector3.up * 0.5f;
        cardData = card;
        impactEffectPrefab = impactEffect;

        StartCoroutine(FallAndExplode());
    }

    private IEnumerator FallAndExplode()
    {
        Vector3 start = transform.position;
        Vector3 end = targetPosition;
        float elapsed = 0f;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fallDuration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        Explode();
    }

    private void Explode()
    {
        // 충돌 이펙트
        if (impactEffectPrefab != null)
        {
            GameObject fx = Instantiate(impactEffectPrefab, targetPosition, Quaternion.identity);

            // sourceCard 설정
            var trigger = fx.GetComponent<AreaEffectTrigger>();
            if (trigger != null)
            {
                trigger.sourceCard = cardData;
            }

            Destroy(fx, 1.5f);
        }

        // 피해 처리
        CardEffectProcessor.ApplyDamageAndStatus(cardData, targetPosition);

        Destroy(gameObject);
    }
}