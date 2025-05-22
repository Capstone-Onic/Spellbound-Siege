using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    public AudioSource audioSource;
    public AudioClip intermissionMusic;
    public AudioClip battleMusic;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    private Coroutine currentFadeRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayIntermissionMusic()
    {
        StartFadeToMusic(intermissionMusic);
    }

    public void PlayBattleMusic()
    {
        StartFadeToMusic(battleMusic);
    }

    private void StartFadeToMusic(AudioClip newClip)
    {
        if (currentFadeRoutine != null)
            StopCoroutine(currentFadeRoutine);

        currentFadeRoutine = StartCoroutine(FadeMusicRoutine(newClip));
    }

    private IEnumerator FadeMusicRoutine(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        // 페이드 아웃
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // 페이드 인
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
        currentFadeRoutine = null;
    }
}
