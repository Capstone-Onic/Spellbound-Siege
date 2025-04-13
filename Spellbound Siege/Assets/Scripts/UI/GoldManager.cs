using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;
    public int currentGold = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"°ñµå È¹µæ: +{amount} ¡æ ÇöÀç °ñµå: {currentGold}");

        // ¾Ö´Ï¸ÞÀÌ¼Ç È¿°ú
        var ui = FindObjectOfType<GoldUIController>();
        if (ui != null)
        {
            LeanTween.cancel(ui.goldText.gameObject); // Áßº¹ ¹æÁö
            ui.goldText.transform.localScale = Vector3.one * 1.1f;
            LeanTween.scale(ui.goldText.gameObject, Vector3.one, 0.25f).setEaseOutBack();
        }
    }
}
