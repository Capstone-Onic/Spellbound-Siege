using UnityEngine;
using Spellbound;

public class TimedAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    /// <summary>
    /// Card에 설정된 statusEffectDuration 만큼만 재생되는 AudioSource를 추가합니다.
    /// 오브젝트 자체의 파괴는 파티클 시스템의 stopAction에 의해 처리됩니다.
    /// </summary>
    public void PlayClip(AudioClip clip, Card cardData, bool loop = false)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
        // Destroy 호출 제거: 이 오브젝트는 이펙트(GameObject)에 붙어 있고,
        // 파티클 시스템이 끝나면 stopAction.Destroy로 함께 사라집니다.
    }
}