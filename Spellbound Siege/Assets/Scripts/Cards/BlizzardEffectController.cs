using System.Collections;
using UnityEngine;
using Spellbound;

/// <summary>
/// ���ڵ� ī�� ��� �� ȭ�� ���� �׵θ� UI�� �����������ϴ� ��Ʈ�ѷ��Դϴ�.
/// ī�� �����Ϳ� ������ ȿ�� ���ӽð��� �ڵ����� ����մϴ�.
/// </summary>
public class BlizzardEffectController : MonoBehaviour
{
    [Header("UI ���� �׵θ� Prefab")]
    [Tooltip("ȭ�� �׵θ��� ���� ȿ���� �� UI �������� �����ϼ���.")]
    public GameObject freezeBorderPrefab;

    [Header("Fade Settings")]
    [Tooltip("���̵� ��(��)")]
    public float fadeInTime = 0.3f;
    [Tooltip("���̵� �ƿ�(��)")]
    public float fadeOutTime = 0.5f;

    /// <summary>
    /// ���ڵ� ī�� ��� �� ȣ���մϴ�.
    /// </summary>
    /// <param name="card">ȿ�� ���ӽð��� ������ ī�� ��ü</param>
    public void PlayFreezeBorder(Card card)
    {
        if (freezeBorderPrefab == null)
        {
            Debug.LogWarning("[BlizzardEffectController] freezeBorderPrefab�� ������� �ʾҽ��ϴ�.");
            return;
        }

        // ī�忡 ������ ���ӽð� ���
        float totalDuration = card.statusEffectDuration;
        if (totalDuration <= 0f)
        {
            Debug.LogWarning("[BlizzardEffectController] ī���� statusEffectDuration�� 0 �����Դϴ�.");
            return;
        }

        Canvas uiCanvas = FindObjectOfType<Canvas>();
        if (uiCanvas == null)
        {
            Debug.LogWarning("[BlizzardEffectController] ���� Canvas�� �����ϴ�. ���� �׵θ� UI�� ������ �� �����ϴ�.");
            return;
        }

        // UI ����
        GameObject borderGO = Instantiate(freezeBorderPrefab, uiCanvas.transform);
        CanvasGroup cg = borderGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // ���̵� ��
        StartCoroutine(Fade(cg, 0f, 1f, fadeInTime, 0f));
        // ���̵� �ƿ�: �� ���ӽð����� ���̵� �ƿ� �ð��� �� ��������
        float fadeOutDelay = Mathf.Max(0f, totalDuration - fadeOutTime);
        StartCoroutine(Fade(cg, 1f, 0f, fadeOutTime, fadeOutDelay));

        // ��ü ���ӽð� �� ����
        Destroy(borderGO, totalDuration);
    }

    private IEnumerator Fade(CanvasGroup cg, float from, float to, float duration, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = to;
    }
}