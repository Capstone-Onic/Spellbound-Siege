using UnityEngine;
using Spellbound;

public class TimedAudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    /// <summary>
    /// Card�� ������ statusEffectDuration ��ŭ�� ����Ǵ� AudioSource�� �߰��մϴ�.
    /// ������Ʈ ��ü�� �ı��� ��ƼŬ �ý����� stopAction�� ���� ó���˴ϴ�.
    /// </summary>
    public void PlayClip(AudioClip clip, Card cardData, bool loop = false)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
        // Destroy ȣ�� ����: �� ������Ʈ�� ����Ʈ(GameObject)�� �پ� �ְ�,
        // ��ƼŬ �ý����� ������ stopAction.Destroy�� �Բ� ������ϴ�.
    }
}