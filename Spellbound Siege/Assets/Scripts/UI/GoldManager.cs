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
        Debug.Log($"골드 획득: +{amount} → 현재 골드: {currentGold}");

        var ui = FindFirstObjectByType<GoldUIController>();
        if (ui != null)
        {
            ui.UpdateGold(currentGold); //  숫자 텍스트 갱신 추가

            LeanTween.cancel(ui.goldText.gameObject);
            ui.goldText.transform.localScale = Vector3.one * 1.1f;
            LeanTween.scale(ui.goldText.gameObject, Vector3.one, 0.25f).setEaseOutBack();
        }
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log($"골드 사용: -{amount} → 남은 골드: {currentGold}");

            var ui = FindFirstObjectByType<GoldUIController>();
            if (ui != null)
            {
                ui.UpdateGold(currentGold); //  숫자 텍스트 갱신 추가

                LeanTween.cancel(ui.goldText.gameObject);
                ui.goldText.transform.localScale = Vector3.one * 0.9f;
                LeanTween.scale(ui.goldText.gameObject, Vector3.one, 0.25f).setEaseOutBack();
            }

            return true;
        }
        else
        {
            Debug.Log("골드 부족으로 유닛 설치 불가");
            return false;
        }
    }
}
