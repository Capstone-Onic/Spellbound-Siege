using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource audioSource;
    public AudioClip upgradeClip;
    public AudioClip sellClip;
    public AudioClip lifeLostClip;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayUpgrade()
    {
        PlayClip(upgradeClip);
    }

    public void PlaySell()
    {
        PlayClip(sellClip);
    }
    public void PlayLifeLost()
    {
        PlayClip(lifeLostClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
