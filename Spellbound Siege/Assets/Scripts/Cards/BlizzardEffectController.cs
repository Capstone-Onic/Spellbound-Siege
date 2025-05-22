using System.Collections;
using UnityEngine;
using Spellbound;

/// <summary>
/// 블리자드 카드 사용 시 화면 얼음 테두리 UI를 생성·제거하는 컨트롤러입니다.
/// 카드 데이터에 설정된 효과 지속시간을 자동으로 사용합니다.
/// </summary>
public class BlizzardEffectController : MonoBehaviour
{
    [Header("UI 얼음 테두리 Prefab")]
    [Tooltip("화면 테두리에 얼음 효과를 줄 UI 프리팹을 연결하세요.")]
    public GameObject freezeBorderPrefab;

    [Header("Fade Settings")]
    [Tooltip("페이드 인(초)")]
    public float fadeInTime = 0.3f;
    [Tooltip("페이드 아웃(초)")]
    public float fadeOutTime = 0.5f;

    /// <summary>
    /// 블리자드 카드 사용 시 호출합니다.
    /// </summary>
    /// <param name="card">효과 지속시간이 설정된 카드 객체</param>
    public void PlayFreezeBorder(Card card)
    {
        if (freezeBorderPrefab == null)
        {
            Debug.LogWarning("[BlizzardEffectController] freezeBorderPrefab이 연결되지 않았습니다.");
            return;
        }

        // 카드에 설정된 지속시간 사용
        float totalDuration = card.statusEffectDuration;
        if (totalDuration <= 0f)
        {
            Debug.LogWarning("[BlizzardEffectController] 카드의 statusEffectDuration이 0 이하입니다.");
            return;
        }

        Canvas uiCanvas = FindObjectOfType<Canvas>();
        if (uiCanvas == null)
        {
            Debug.LogWarning("[BlizzardEffectController] 씬에 Canvas가 없습니다. 얼음 테두리 UI를 생성할 수 없습니다.");
            return;
        }

        // UI 생성
        GameObject borderGO = Instantiate(freezeBorderPrefab, uiCanvas.transform);
        CanvasGroup cg = borderGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // 페이드 인
        StartCoroutine(Fade(cg, 0f, 1f, fadeInTime, 0f));
        // 페이드 아웃: 총 지속시간에서 페이드 아웃 시간을 뺀 지점부터
        float fadeOutDelay = Mathf.Max(0f, totalDuration - fadeOutTime);
        StartCoroutine(Fade(cg, 1f, 0f, fadeOutTime, fadeOutDelay));

        // 전체 지속시간 후 제거
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