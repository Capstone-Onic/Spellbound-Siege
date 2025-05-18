using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ManaManager : MonoBehaviour
{
    public static ManaManager Instance;

    [Header("Mana Settings")]
    public int maxMana = 10;
    public int currentMana = 0;
    public float manaRegenInterval = 5f;

    [Header("UI References")]
    public Image manaGaugeFill;
    public RectTransform manaGaugeParent;
    public Image manaImage;
    public TextMeshProUGUI manaText;
    public GameObject manaWarningText;

    [Header("Gauge Animation")]
    public float gaugeSpeed = 4f;
    private float currentFillAmount = 0f;

    [Header("Visual FX")]
    public GameObject manaGlowEffect;  // Glow 이펙트 오브젝트

    private Coroutine regenRoutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartRegen()
    {
        if (regenRoutine == null)
            regenRoutine = StartCoroutine(AutoRegenMana());
    }

    void Update()
    {
        float target = Mathf.Clamp01((float)currentMana / maxMana);
        currentFillAmount = Mathf.Lerp(currentFillAmount, target, Time.deltaTime * gaugeSpeed);

        if (manaGaugeFill != null)
            manaGaugeFill.fillAmount = currentFillAmount;
    }

    public bool ConsumeMana(int amount)
    {
        if (currentMana < amount)
            return false;

        currentMana -= amount;
        UpdateManaUI();
        return true;
    }

    public void GainMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        UpdateManaUI();
    }

    public void UpdateManaUI()
    {
        if (manaText != null)
            manaText.text = currentMana.ToString();

        if (manaGaugeFill != null)
        {
            float percent = Mathf.Clamp01((float)currentMana / maxMana);
            manaGaugeFill.fillAmount = percent;
        }
    }

    private IEnumerator AutoRegenMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(manaRegenInterval);
            if (currentMana < maxMana)
            {
                currentMana++;
                UpdateManaUI();
                PlayManaGlow();       // 회복 시 Glow
                AnimateManaText();    // 회복 시 숫자 팝업
            }
        }
    }

    public void ShowManaWarning()
    {
        if (manaWarningText == null) return;

        StopCoroutine(nameof(HideManaWarning));
        manaWarningText.SetActive(true);
        StartCoroutine(HideManaWarning());
    }

    private IEnumerator HideManaWarning()
    {
        yield return new WaitForSeconds(1f);
        manaWarningText.SetActive(false);
    }

    // Glow 이펙트 실행
    private void PlayManaGlow()
    {
        if (manaGlowEffect == null) return;

        manaGlowEffect.SetActive(true);
        CancelInvoke(nameof(HideManaGlow));
        Invoke(nameof(HideManaGlow), 0.3f);
    }

    private void HideManaGlow()
    {
        manaGlowEffect.SetActive(false);
    }

    // 숫자 커졌다 작아지는 팝업 애니메이션
    private void AnimateManaText()
    {
        if (manaText == null) return;

        LeanTween.cancel(manaText.gameObject);
        RectTransform rt = manaText.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;

        rt.localScale = originalScale;
        LeanTween.scale(rt, originalScale * 1.4f, 0.1f).setEaseOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.scale(rt, originalScale, 0.2f).setEaseInOutQuad();
            });
    }

    public void StopRegen()
    {
        if (regenRoutine != null)
        {
            StopCoroutine(regenRoutine);
            regenRoutine = null;
        }
    }
}